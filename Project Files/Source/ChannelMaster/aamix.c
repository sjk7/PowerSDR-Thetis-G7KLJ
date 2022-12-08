
// This is an independent project of an individual developer. Dear PVS-Studio,
// please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java:
// http://www.viva64.com
/*  aamix.c

This file is part of a program that implements a Software-Defined Radio.

Copyright (C) 2014 Warren Pratt, NR0V

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

The author can be reached by email at

warren@wpratt.com

*/
#include "cmcomm.h"
#include <stdint.h> // uint64_t

#ifndef PRIO_THRD_DEFINED
#define PRIO_THRD_DEFINED

static inline HANDLE prioritise_thread_max() {

    DWORD taskIndex = 0;
    HANDLE hTask = AvSetMmThreadCharacteristics(TEXT("Pro Audio"), &taskIndex);
    if (hTask != 0) {

        BOOL ok = AvSetMmThreadPriority(hTask, AVRT_PRIORITY_CRITICAL);
        assert(ok);

    } else {
        // assert("Why did setting thread priority fail?" == 0);
        const DWORD dw = GetLastError();
        if (dw == 1552) { // the specified thread is already joining a task
            // assert(0);

        } else {
            SetThreadPriority(
                GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL);
            fprintf(stderr,
                "I don't like this, falling back to "
                "THREAD_PRIORITY_TIME_CRITICAL");
            fflush(stderr);
        }
    }
    return hTask;
}

static inline BOOL prioritise_thread_cleanup(HANDLE h) {
    BOOL ret = AvRevertMmThreadCharacteristics(h);
    if (ret == 0) {
        DWORD dw = GetLastError();
        assert(0);
        fprintf(stderr,
            "Failed to clean up thread priority, with error code: %ld\n",
            (int)dw);
    }

    return ret;
}
#endif

#define MAX_EXT_AAMIX (4) // maximum number of AAMIXs called from outside wdsp
__declspec(align(16)) AAMIX
    paamix[MAX_EXT_AAMIX]; // array of pointers for AAMIXs used EXTERNAL to wdsp

#define MAX_POSS_MIXER_INPUTS 32
typedef struct flagged_info_type {
    volatile DWORD since_flagged_longest;
    unsigned long num_handles;
    volatile DWORD longest_ago;
    volatile int longest_ago_index;
    volatile int shortest_ago_index;
    volatile DWORD shortest_ago;
    volatile DWORD times_ago[MAX_POSS_MIXER_INPUTS];
    volatile DWORD when_signalled[MAX_POSS_MIXER_INPUTS];
    volatile int mixer_id;
} flagged_info_t;

static flagged_info_t sort_when_inputs_flagged(AAMIX a) {

    DWORD temp_shortest_ago = 10000;
    DWORD temp_ago = 0;
    flagged_info_t retval = {0};
    retval.mixer_id = a->id;
    retval.num_handles = MAX_POSS_MIXER_INPUTS;
    // sort these buggers
    for (unsigned int i = 0; i < retval.num_handles; ++i) {

        if (a->when_ready_flagged[i]) {

            volatile DWORD ago = timeGetTime() - a->when_ready_flagged[i];
            retval.times_ago[i] = ago;
            retval.when_signalled[i] = a->when_ready_flagged[i];
            if (ago >= temp_ago) {
                retval.longest_ago = ago;
                temp_ago = ago;
                retval.longest_ago_index = i;
            }
            if (ago <= temp_shortest_ago) {
                retval.shortest_ago = ago;
                temp_shortest_ago = ago;
                retval.shortest_ago_index = i;
            }
        }
    }

    return retval;
}

static volatile uint32_t ctr = 0;
void mix_main(void* pargs) {
    HANDLE hpri = prioritise_thread_max();

    AAMIX a = (AAMIX)pargs;

    ctr = 0;
    while (_InterlockedAnd(&a->run, 1)) {
#ifdef DEBUG_TIMINGS
        DWORD dw1 = timeGetTime();
#endif
        DWORD dwWait = WaitForMultipleObjects(a->nactive, a->Aready, TRUE, 500);

        if (xaamix(a) < 0) {
            break;
        }
        if (dwWait == WAIT_TIMEOUT) {
            continue;
        }

#ifdef DEBUG_TIMINGS
        DWORD dw2 = timeGetTime();
        volatile DWORD took = dw2 - dw1;
        if (took > 10 && ctr++ > 50) {
            printf("***********************\nAnother hold-up! %ld "
                   "ms.\n********************\n\n",
                (int)took);
            volatile flagged_info_t fi = sort_when_inputs_flagged(a);
            assert(fi.shortest_ago_index >= 0 && fi.longest_ago_index >= 0);
            printf("Was held up waiting for input for mixer with id: %ld, with "
                   "index: %ld\n",
                fi.mixer_id, (int)fi.shortest_ago_index);
            printf("INFO: The earliest input was flagged %ld ms ago,\nmeaning "
                   "the difference between flagged times is: %ld [idx %ld] - "
                   "%ld [idx %ld] \n",
                (int)fi.longest_ago, (int)(fi.shortest_ago - fi.longest_ago),
                fi.shortest_ago_index, fi.shortest_ago_index,
                fi.longest_ago_index);
            fflush(stdout);
            assert(fi.longest_ago_index >= 0 && fi.shortest_ago_index >= 0);
        }
#endif

        (*a->Outbound)(a->outbound_id, a->outsize, a->out);
        // WriteAudio (30.0, 48000, a->outsize, a->out, 3);
    }

    if (hpri && hpri != INVALID_HANDLE_VALUE) prioritise_thread_cleanup(hpri);
}

// G7KLJ:
// added to ensure correct prioritise_thread_max cleanup;
void mix_main_proxy(void* pargs) {
    mix_main(pargs);
}

void start_mixthread(AAMIX a) {
    _beginthread(mix_main_proxy, 0, (void*)a);
}

enum _slew { BEGIN = 0, DELAYUP, UPSLEW, ON, DELAYDOWN, DOWNSLEW, ZERO, OFF };

void create_aaslew(AAMIX a) {
    int i;
    double delta, theta;
    a->slew.ustate = BEGIN;
    a->slew.dstate = BEGIN;
    a->slew.ucount = 0;
    a->slew.dcount = 0;
    a->slew.ndelup = (int)(a->slew.tdelayup * a->outrate);
    a->slew.ndeldown = (int)(a->slew.tdelaydown * a->outrate);
    a->slew.ntup = (int)(a->slew.tslewup * a->outrate);
    a->slew.ntdown = (int)(a->slew.tslewdown * a->outrate);
    a->slew.cup = (double*)malloc0((a->slew.ntup + 1) * sizeof(double));
    a->slew.cdown = (double*)malloc0((a->slew.ntdown + 1) * sizeof(double));

    delta = PI / (double)a->slew.ntup;
    theta = 0.0;
    for (i = 0; i <= a->slew.ntup; i++) {
        a->slew.cup[i] = 0.5 * (1.0 - cos(theta));
        theta += delta;
    }

    delta = PI / (double)a->slew.ntdown;
    theta = 0.0;
    for (i = 0; i <= a->slew.ntdown; i++) {
        a->slew.cdown[i] = 0.5 * (1 + cos(theta));
        theta += delta;
    }

    a->slew.uwait = CreateSemaphore(0, 0, 1, 0);
    a->slew.dwait = CreateSemaphore(0, 0, 1, 0);
    InterlockedBitTestAndReset(&a->slew.uflag, 0);
    InterlockedBitTestAndReset(&a->slew.dflag, 0);

    a->slew.utimeout = (int)(1000.0 * (a->slew.tdelayup + a->slew.tslewup)) + 2;
    a->slew.dtimeout
        = (int)(1000.0 * (a->slew.tdelaydown + a->slew.tslewdown)) + 2;
}

void destroy_aaslew(AAMIX a) {
    CloseHandle(a->slew.dwait);
    CloseHandle(a->slew.uwait);
    _aligned_free(a->slew.cdown);
    _aligned_free(a->slew.cup);
}

void flush_aaslew(AAMIX a) {
    a->slew.ustate = BEGIN;
    a->slew.dstate = BEGIN;
    a->slew.ucount = 0;
    a->slew.dcount = 0;
    InterlockedBitTestAndReset(&a->slew.uflag, 0);
    InterlockedBitTestAndReset(&a->slew.dflag, 0);
}

void* create_aamix(int id, int outbound_id, int ringinsize, int outsize,
    int ninputs, long active, long what, double volume, int ring_size,
    int* inrates, int outrate,
    void (*Outbound)(int id, int nsamples, double* buff), double tdelayup,
    double tslewup, double tdelaydown, double tslewdown) {
    int i;
    AAMIX a = (AAMIX)malloc0(sizeof(aamix));
    a->id = id;
    a->outbound_id = outbound_id;
    a->ringinsize = ringinsize;
    a->outsize = outsize;
    a->ninputs = ninputs;
    a->active = active;
    a->what = what;
    a->volume = volume;
    a->rsize = ring_size;
    a->outrate = outrate;
    a->Outbound = Outbound;
    a->slew.tdelayup = tdelayup;
    a->slew.tslewup = tslewup;
    a->slew.tdelaydown = tdelaydown;
    a->slew.tslewdown = tslewdown;
    for (i = 0; i < a->ninputs; i++)
        a->ring[i] = (double*)malloc0(a->rsize * sizeof(WDSP_COMPLEX));
    a->out = (double*)malloc0(a->outsize * sizeof(WDSP_COMPLEX));
    a->nactive = 0;
    for (i = 0; i < a->ninputs; i++) {
        a->vol[i] = 1.0;
        a->tvol[i] = a->volume;
        a->inidx[i] = 0;
        a->outidx[i] = 0;
        a->unqueuedsamps[i] = 0;
        a->Ready[i] = CreateSemaphore(0, 0, 1000, 0);
        InitializeCriticalSectionAndSpinCount(&a->cs_in[i], 2500);
        if (_InterlockedAnd(&a->active, 0xffffffff) & (1 << i)) {

            int idx = a->nactive++;

#ifdef DEBUG_TIMINGS

            a->when_ready_flagged[idx] = timeGetTime();
#endif

            a->Aready[idx] = a->Ready[i];
            InterlockedBitTestAndSet(&a->accept[i], 0);
        } else
            InterlockedBitTestAndReset(&a->accept[i], 0);
    }
    for (i = 0; i < a->ninputs; i++) // resamplers
    {
        int run, size;
        a->inrate[i] = inrates[i];
        // inrate & outrate must be related by an integer multiple or
        // sub-multiple
        if (a->inrate[i] != a->outrate)
            run = 1;
        else
            run = 0;
        if (a->inrate[i] > a->outrate)
            size = a->ringinsize * (a->inrate[i] / a->outrate);
        else
            size = a->ringinsize / (a->outrate / a->inrate[i]);
        a->resampbuff[i]
            = (double*)malloc0(a->ringinsize * sizeof(WDSP_COMPLEX));
        a->rsmp[i] = create_resample(run, size, 0, a->resampbuff[i],
            a->inrate[i], a->outrate, 0.0, 0, 1.0);
    }
    InitializeCriticalSectionAndSpinCount(&a->cs_out, 2500);
    // slew
    create_aaslew(a);
    // slew_end
    InterlockedBitTestAndSet(&a->run, 0);
    if (a->nactive) start_mixthread(a);
    if (a->id >= 0) paamix[id] = a;
    return (void*)a;
}

void destroy_aamix(void* ptr, int id) {
    int i;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    InterlockedBitTestAndReset(&a->run, 0);
    for (i = 0; i < a->ninputs; i++) ReleaseSemaphore(a->Ready[i], 1, 0);
    Sleep(2);
    DeleteCriticalSection(&a->cs_out);
    for (i = 0; i < a->ninputs; i++) {
        destroy_resample(a->rsmp[i]);
        _aligned_free(a->resampbuff[i]);
        DeleteCriticalSection(&a->cs_in[i]);
        CloseHandle(a->Ready[i]);
    }
    _aligned_free(a->out);
    for (i = 0; i < a->ninputs; i++) _aligned_free(a->ring[i]);
    // slew
    destroy_aaslew(a);
    // slew_end
    _aligned_free(a);
}

// loads data from a buffer into an audio mixer ring
#ifdef DEBUG_TIMINGS
static DWORD last_times[32];
static uint32_t times_ctr = 0;
#endif

void xMixAudio(void* ptr, int id, int stream, double* data) {

    DWORD time_enter = timeGetTime();
    int first, second, n;
    double* indata = 0;
    AAMIX a = 0;

#ifdef DEBUG_TIMINGS
    if (last_times[stream] && times_ctr++ > 5000) {
        DWORD since = time_enter - last_times[stream];
        if (since > 10 && since < 1000) {
            fprintf(stderr,
                "xMixAudio: long time %ld ms between calls, after being called "
                "%ld times, for stream %ld\n",
                (int)since, times_ctr, stream);
        }
    }
#endif

    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    if (_InterlockedAnd(&a->accept[stream], 1)) {
        EnterCriticalSection(&a->cs_in[stream]);
        if (a->rsmp[stream]->run) {
            a->rsmp[stream]->in = data;
            indata = a->resampbuff[stream];
            xresample(a->rsmp[stream]);
        } else
            indata = data;
        if (a->ringinsize > (a->rsize - a->inidx[stream])) {
            first = a->rsize - a->inidx[stream];
            second = a->ringinsize - first;
        } else {
            first = a->ringinsize;
            second = 0;
        }
        memcpy(a->ring[stream] + 2 * a->inidx[stream], indata,
            first * sizeof(WDSP_COMPLEX));
        memcpy(
            a->ring[stream], indata + 2 * first, second * sizeof(WDSP_COMPLEX));

        if ((a->unqueuedsamps[stream] += a->ringinsize) >= a->outsize) {
            n = a->unqueuedsamps[stream] / a->outsize;
#ifdef DEBUG_TIMINGS
            a->when_ready_flagged[stream] = timeGetTime();
#endif
            ReleaseSemaphore(a->Ready[stream], n, 0);
            a->unqueuedsamps[stream] -= n * a->outsize;
        }
        if ((a->inidx[stream] += a->ringinsize) >= a->rsize)
            a->inidx[stream] -= a->rsize;
        LeaveCriticalSection(&a->cs_in[stream]);
#ifdef DEBUG_TIMINGS
        last_times[stream] = timeGetTime();
#endif
    }
}

void upslew(AAMIX a) {
    int i;
    double I, Q;
    double* pin = a->out;
    double* pout = a->out;
    for (i = 0; i < a->outsize; i++) {
        I = pin[2 * i + 0];
        Q = pin[2 * i + 1];
        switch (a->slew.ustate) {
            case BEGIN:
                pout[2 * i + 0] = 0.0;
                pout[2 * i + 1] = 0.0;
                if ((I != 0.0) || (Q != 0.0)) {
                    if (a->slew.ndelup > 0) {
                        a->slew.ustate = DELAYUP;
                        a->slew.ucount = a->slew.ndelup;
                    } else if (a->slew.ntup > 0) {
                        a->slew.ustate = UPSLEW;
                        a->slew.ucount = a->slew.ntup;
                    } else
                        a->slew.ustate = ON;
                }
                break;
            case DELAYUP:
                pout[2 * i + 0] = 0.0;
                pout[2 * i + 1] = 0.0;
                if (a->slew.ucount-- == 0) {
                    if (a->slew.ntup > 0) {
                        a->slew.ustate = UPSLEW;
                        a->slew.ucount = a->slew.ntup;
                    } else
                        a->slew.ustate = ON;
                }
                break;
            case UPSLEW:
                pout[2 * i + 0]
                    = I * a->slew.cup[a->slew.ntup - a->slew.ucount];
                pout[2 * i + 1]
                    = Q * a->slew.cup[a->slew.ntup - a->slew.ucount];
                if (a->slew.ucount-- == 0) a->slew.ustate = ON;
                break;
            case ON:
                pout[2 * i + 0] = I;
                pout[2 * i + 1] = Q;
                if (i == a->outsize - 1) {
                    a->slew.ustate = BEGIN;
                    InterlockedBitTestAndReset(&a->slew.uflag, 0);
                    ReleaseSemaphore(a->slew.uwait, 1, 0);
                }
                break;
        }
    }
}

void downslew(AAMIX a) {
    int i;
    double I, Q;
    double* pin = a->out;
    double* pout = a->out;
    for (i = 0; i < a->outsize; i++) {
        I = pin[2 * i + 0];
        Q = pin[2 * i + 1];
        switch (a->slew.dstate) {
            case BEGIN:
                pout[2 * i + 0] = I;
                pout[2 * i + 1] = Q;
                if (a->slew.ndeldown > 0) {
                    a->slew.dstate = DELAYDOWN;
                    a->slew.dcount = a->slew.ndeldown;
                } else if (a->slew.ntdown > 0) {
                    a->slew.dstate = DOWNSLEW;
                    a->slew.dcount = a->slew.ntdown;
                } else {
                    a->slew.dstate = ZERO;
                    a->slew.dcount = a->outsize;
                }
                break;
            case DELAYDOWN:
                pout[2 * i + 0] = I;
                pout[2 * i + 1] = Q;
                if (a->slew.dcount-- == 0) {
                    if (a->slew.ntdown > 0) {
                        a->slew.dstate = DOWNSLEW;
                        a->slew.dcount = a->slew.ntdown;
                    } else {
                        a->slew.dstate = ZERO;
                        a->slew.dcount = a->outsize;
                    }
                }
                break;
            case DOWNSLEW:
                pout[2 * i + 0]
                    = I * a->slew.cdown[a->slew.ntdown - a->slew.dcount];
                pout[2 * i + 1]
                    = Q * a->slew.cdown[a->slew.ntdown - a->slew.dcount];
                if (a->slew.dcount-- == 0) {
                    a->slew.dstate = ZERO;
                    a->slew.dcount = a->outsize;
                }
                break;
            case ZERO:
                pout[2 * i + 0] = 0.0;
                pout[2 * i + 1] = 0.0;
                if (a->slew.dcount-- == 0) a->slew.dstate = OFF;
                break;
            case OFF:
                pout[2 * i + 0] = 0.0;
                pout[2 * i + 1] = 0.0;
                if (i == a->outsize - 1) {
                    a->slew.dstate = BEGIN;
                    InterlockedBitTestAndReset(&a->slew.dflag, 0);
                    ReleaseSemaphore(a->slew.dwait, 1, 0);
                }
                break;
        }
    }
}

// pulls data from audio rings and mixes with output
// G7KLJ: we now return < 0 to end the thread gracefully,
// otherwise the Pro Audio thread priority API does not get cleaned up when the
// thread exits, and we end up with below optimum priority when the resources
// run out (usually the second time the radio is started).
int xaamix(AAMIX a) {
    int i, j;
    int what, mask, idx;

    EnterCriticalSection(&a->cs_out);

    if (!_InterlockedAnd(&a->run, 1)) {
        LeaveCriticalSection(&a->cs_out);
        //_endthread();
        // no endthread! doesn't clean up after itself!
        return -1;
    }
    memset(a->out, 0, a->outsize * sizeof(WDSP_COMPLEX));
    what = _InterlockedAnd(&a->what, 0xffffffff)
        & _InterlockedAnd(&a->active, 0xffffffff);
    i = 0;

    while (what != 0) {
        mask = 1 << i;
        if ((mask & what) != 0) {
            idx = a->outidx[i];
            for (j = 0; j < a->outsize; j++) {
                a->out[2 * j + 0] += a->tvol[i] * a->ring[i][2 * idx + 0];
                a->out[2 * j + 1] += a->tvol[i] * a->ring[i][2 * idx + 1];
                if (++idx == a->rsize) idx = 0;
            }
            what &= ~mask;
        }
        i++;
    }
    for (i = 0; i < a->ninputs; i++)
        if (_InterlockedAnd(&a->accept[i], 1))
            if ((a->outidx[i] += a->outsize) >= a->rsize)
                a->outidx[i] -= a->rsize;
    if (_InterlockedAnd(&a->slew.uflag, 1)) upslew(a);
    if (_InterlockedAnd(&a->slew.dflag, 1)) downslew(a);

    LeaveCriticalSection(&a->cs_out);
    return NOERROR;
}

void flush_mix_ring(AAMIX a, int stream) {
    memset(a->ring[stream], 0, a->rsize * sizeof(WDSP_COMPLEX));
    a->inidx[stream] = 0;
    a->outidx[stream] = 0;
    a->unqueuedsamps[stream] = 0;
    while (!WaitForSingleObject(a->Ready[stream], 1))
        ;
    flush_resample(a->rsmp[stream]);
}

void close_mixer(AAMIX a) {
    int i;
    InterlockedBitTestAndSet(
        &a->slew.dflag, 0); // set a bit telling downslew to proceed
    WaitForSingleObject(
        a->slew.dwait, a->slew.dtimeout); // block until downslew is complete or
                                          // timeout if data is not flowing
    InterlockedBitTestAndReset(&a->slew.dflag, 0);
    for (i = 0; i < a->ninputs; i++)
        InterlockedBitTestAndReset(
            &a->accept[i], 0); // shut the gates to prevent new infusions
    Sleep(1); // if a thread has just passed the gate, allow time to get cs_in
              // and get through
    for (i = 0; i < a->ninputs; i++)
        EnterCriticalSection(
            &a->cs_in[i]); // wait until the current infusions are all finished
    EnterCriticalSection(
        &a->cs_out); // block the mixer thread at the beginning of xaamix()
    Sleep(25); // wait for thread to arrive at the top of the main() loop
    InterlockedBitTestAndReset(&a->run, 0); // set a trap for the mixer thread
    for (i = 0; i < a->ninputs; i++) {

        ReleaseSemaphore(
            a->Ready[i], 1, 0); // be sure the mixer thread can pass
                                // WaitForMultipleObjects in main()
    }

    LeaveCriticalSection(
        &a->cs_out); // let the thread pass to the trap in xaamix()
    Sleep(2); // wait for the mixer thread to die
    for (i = 0; i < a->ninputs; i++)
        flush_mix_ring(a, i); // restore rings to pristine condition
}

void open_mixer(AAMIX a) {
    int i;
    InterlockedBitTestAndSet(
        &a->slew.uflag, 0); // set a bit telling upslew to proceed (when there
                            // are samples flowing)
    InterlockedBitTestAndSet(&a->run, 0); // remove the mixer thread trap
    if (a->nactive)
        start_mixthread(a); // start the mixer thread if there's anything to mix
    for (i = a->ninputs - 1; i >= 0; i--)
        LeaveCriticalSection(&a->cs_in[i]); // enable xMixAudio() processing
    for (i = a->ninputs - 1; i >= 0; i--)
        if (_InterlockedAnd(&a->active, 0xffffffff) & (1 << i))
            InterlockedBitTestAndSet(&a->accept[i],
                0); // open the xMixAudio() gates for active streams
    WaitForSingleObject(a->slew.uwait,
        a->slew.utimeout); // block on semaphore until upslew complete, or until
                           // timeout if no data flow
}

/********************************************************************************************************
 *																										*
 *									         MIXER PROPERTIES
 **
 *																										*
 ********************************************************************************************************/

PORT const char* build_date() {
    return __DATE__;
}
void SetAAudioMixOutputPointer(
    void* ptr, int id, void (*Outbound)(int id, int nsamples, double* buff)) {
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    a->Outbound = Outbound;
}

PORT void SetAAudioMixState(void* ptr, int id, int stream, int state) {
    int i;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    if (((_InterlockedAnd(&a->active, 0xffffffff) >> stream) & 1) != state) {
        close_mixer(a);
        if (state)
            _InterlockedOr(&a->active, 1 << stream); // set stream active
        else
            _InterlockedAnd(&a->active, ~(1 << stream)); // set stream inactive
        a->nactive = 0;
        for (i = 0; i < a->ninputs; i++)
            if (_InterlockedAnd(&a->active, 0xffffffff) & (1 << i)) {
                int idx = a->nactive++;
#ifdef DEBUG_TIMINGS

                a->when_ready_flagged[idx] = timeGetTime();
#endif
                a->Aready[idx] = a->Ready[i];
                InterlockedBitTestAndSet(&a->accept[i], 0);
            } else
                InterlockedBitTestAndReset(&a->accept[i], 0);
        open_mixer(a);
    }
}

// SetAAudioMixStates() is an alternative to SetAAudioMixState() that can be
// used to set multiple mix states with only a single call.  'streams' has one
// bit per mix state that you want to set and 'states' has one bit specifying
// the state of each stream that you want to set.  
// For example, if you want to set the state of streams 0 and 3 to 1 and 0, 
// respectively: streams = 9 [...1001] states =  1 [...0001]
PORT void SetAAudioMixStates(void* ptr, int id, int streams, int states) {
    int i;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    if ((_InterlockedAnd(&a->active, 0xffffffff) & streams)
        != (states & streams)) {
        close_mixer(a);
        for (i = 0; i < a->ninputs; i++)
            if ((streams >> i) & 1)
                if ((states >> i) & 1)
                    _InterlockedOr(&a->active, 1 << i); // set stream active
                else
                    _InterlockedAnd(
                        &a->active, ~(1 << i)); // set stream inactive

        a->nactive = 0;
        for (i = 0; i < a->ninputs; i++)
            if (_InterlockedAnd(&a->active, 0xffffffff) & (1 << i)) {
                int myactive = a->nactive++;
#ifdef DEBUG_TIMINGS

                a->when_ready_flagged[myactive] = timeGetTime();
#endif
                a->Aready[myactive] = a->Ready[i];
                InterlockedBitTestAndSet(&a->accept[i], 0);
            } else
                InterlockedBitTestAndReset(&a->accept[i], 0);
        open_mixer(a);
    }
}

PORT void SetAAudioMixWhat(void* ptr, int id, int stream, int state) {
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    if (state)
        InterlockedBitTestAndSet(&a->what, stream); // turn on mixing
    else
        InterlockedBitTestAndReset(&a->what, stream); // turn off mixing
}

PORT void SetAAudioMixVolume(void* ptr, int id, double volume) {
    int i;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    EnterCriticalSection(&a->cs_out);
    a->volume = volume;
    for (i = 0; i < 32; i++) {
        a->tvol[i] = a->volume * a->vol[i];
    }
    LeaveCriticalSection(&a->cs_out);
}

PORT void SetAAudioMixVol(void* ptr, int id, int stream, double vol) {
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    EnterCriticalSection(&a->cs_out);
    a->vol[stream] = vol;
    a->tvol[stream] = a->vol[stream] * a->volume;
    LeaveCriticalSection(&a->cs_out);
}

void SetAAudioRingInsize(void* ptr, int id, int size) {
    int i, rs_size;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    close_mixer(a);
    a->ringinsize = size;
    for (i = 0; i < a->ninputs; i++) { // inrate & outrate must be related by an
                                       // integer multiple or sub-multiple
        if (a->inrate[i] > a->outrate)
            rs_size = a->ringinsize * (a->inrate[i] / a->outrate);
        else
            rs_size = a->ringinsize / (a->outrate / a->inrate[i]);
        a->rsmp[i]->size = rs_size;
        _aligned_free(a->resampbuff[i]);
        a->resampbuff[i]
            = (double*)malloc0(a->ringinsize * sizeof(WDSP_COMPLEX));
        a->rsmp[i]->out = a->resampbuff[i];
    }
    open_mixer(a);
}

void SetAAudioRingOutsize(void* ptr, int id, int size) {
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    close_mixer(a);
    a->outsize = size;
    _aligned_free(a->out);
    a->out = (double*)malloc0(a->outsize * sizeof(WDSP_COMPLEX));
    open_mixer(a);
}

void SetAAudioOutRate(void* ptr, int id, int rate) {
    int i;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    close_mixer(a);
    a->outrate = rate;
    for (i = 0; i < a->ninputs; i++) // resamplers
    {
        int run, size;
        destroy_resample(a->rsmp[i]);
        _aligned_free(a->resampbuff[i]);
        // inrate & outrate must be related by an integer multiple or
        // sub-multiple
        if (a->inrate[i] != a->outrate)
            run = 1;
        else
            run = 0;
        if (a->inrate[i] > a->outrate)
            size = a->ringinsize * (a->inrate[i] / a->outrate);
        else
            size = a->ringinsize / (a->outrate / a->inrate[i]);
        a->resampbuff[i]
            = (double*)malloc0(a->ringinsize * sizeof(WDSP_COMPLEX));
        a->rsmp[i] = create_resample(run, size, 0, a->resampbuff[i],
            a->inrate[i], a->outrate, 0.0, 0, 1.0);
        a->rsmp[i]->out = a->resampbuff[i];
    }
    open_mixer(a);
}

void SetAAudioStreamRate(void* ptr, int id, int mixinid,
    int rate) { // NOTE: you must set the stream state to INACTIVE before using
                // this function!
    int run, size;
    AAMIX a;
    if (ptr == 0)
        a = paamix[id];
    else
        a = (AAMIX)ptr;
    a->inrate[mixinid] = rate;
    destroy_resample(a->rsmp[mixinid]);
    // inrate & outrate must be related by an integer multiple or sub-multiple
    if (a->inrate[mixinid] != a->outrate)
        run = 1;
    else
        run = 0;
    if (a->inrate[mixinid] > a->outrate)
        size = a->ringinsize * (a->inrate[mixinid] / a->outrate);
    else
        size = a->ringinsize / (a->outrate / a->inrate[mixinid]);
    a->rsmp[mixinid] = create_resample(run, size, 0, a->resampbuff[mixinid],
        a->inrate[mixinid], a->outrate, 0.0, 0, 1.0);
}
