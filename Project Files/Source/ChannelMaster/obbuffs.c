// This is an independent project of an individual developer. Dear PVS-Studio,
// please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java:
// http://www.viva64.com
/*  obbuffs.c

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

#include "obbuffs.h"
#include "comm.h"

struct _obpointers {
    OBB pcbuff[numRings];
    OBB pdbuff[numRings];
    OBB pebuff[numRings];
    OBB pfbuff[numRings];
} obp;

void start_obthread(int id) {
#pragma warning(disable : 4312)
    _beginthread(ob_main, 0, (void*)(ptrdiff_t)id);
#pragma warning(default : 4312)
}

void create_obbuffs(int id, int accept, int max_insize, int outsize) {

    OBB existing = obp.pcbuff[id];
    if (existing) {
        // return; // KLJ: prevents multiple instances of thread when radio
        // turned on more than once in current session.
        destroy_obbuffs(id);
    }

    OBB a = (OBB)calloc(1, sizeof(obb));
    assert(a);

    obp.pcbuff[id] = obp.pdbuff[id] = obp.pebuff[id] = obp.pfbuff[id] = a;
    a->id = id;
    a->accept = accept;
    a->run = 1;
    a->max_in_size = max_insize;
    a->r1_outsize = outsize;
    if (a->r1_outsize > a->max_in_size)
        a->r1_size = a->r1_outsize;
    else
        a->r1_size = a->max_in_size;
    a->r1_active_buffsize = OBB_MULT * a->r1_size;
    a->r1_baseptr
        = (double*)calloc(a->r1_active_buffsize, sizeof(WDSP_COMPLEX));
    a->r1_inidx = 0;
    a->r1_outidx = 0;
    a->r1_unqueuedsamps = 0;
    a->Sem_BuffReady = CreateSemaphore(0, 0, 1000, 0);
    InitializeCriticalSectionAndSpinCount(&a->csIN, 2500);
    InitializeCriticalSectionAndSpinCount(&a->csOUT, 2500);
    a->out = (double*)calloc(obMAXSIZE, sizeof(WDSP_COMPLEX));
    start_obthread(id);
}

void destroy_obbuffs(int id) {
    OBB a = obp.pcbuff[id];
    if (obp.pcbuff[0] == NULL) return;
    InterlockedBitTestAndReset(&a->accept, 0);
    EnterCriticalSection(&a->csIN);
    EnterCriticalSection(&a->csOUT);
    Sleep(25);
    InterlockedBitTestAndReset(&a->run, 0);
    ReleaseSemaphore(a->Sem_BuffReady, 1, 0);
    LeaveCriticalSection(&a->csOUT);
    Sleep(2);
    DeleteCriticalSection(&a->csOUT);
    DeleteCriticalSection(&a->csIN);
    CloseHandle(a->Sem_BuffReady);
    free(a->out);
    free(a->r1_baseptr);
    free(a);
}

void flush_obbuffs(int id) {
    OBB a = obp.pfbuff[id];
    memset(a->r1_baseptr, 0, a->r1_active_buffsize * sizeof(WDSP_COMPLEX));
    a->r1_inidx = 0;
    a->r1_outidx = 0;
    a->r1_unqueuedsamps = 0;
    while (!WaitForSingleObject(a->Sem_BuffReady, 1))
        ;
}

DWORD last_outbound_time = 0;
PORT void OutBound(int id, int nsamples, double* in) {
    int n;
    int first, second;
    OBB a = obp.pebuff[id];

    if (last_outbound_time) {
        volatile DWORD took = timeGetTime() - last_outbound_time;
        if (took > 20) {
#ifdef _DEBUG
            printf("What's the hold-up? %ld\n", (int)took);
#endif
        }
    }
    DWORD d1 = timeGetTime();
    if (_InterlockedAnd(&a->accept, 1)) {
        EnterCriticalSection(&a->csIN);
        if (nsamples > (a->r1_active_buffsize - a->r1_inidx)) {
            first = a->r1_active_buffsize - a->r1_inidx;
            second = nsamples - first;
        } else {
            first = nsamples;
            second = 0;
        }
        memcpy(
            a->r1_baseptr + 2 * a->r1_inidx, in, first * sizeof(WDSP_COMPLEX));
        memcpy(a->r1_baseptr, in + 2 * first, second * sizeof(WDSP_COMPLEX));

        if ((a->r1_unqueuedsamps += nsamples) >= a->r1_outsize) {
            n = a->r1_unqueuedsamps / a->r1_outsize;
            DWORD d2 = timeGetTime();
            volatile DWORD took = d2 - d1;
            if (took > 10) {
                printf("took ages: %ld\n", (int)took);
            }
            a->sem_buff_flagged_when = timeGetTime();
            ReleaseSemaphore(a->Sem_BuffReady, n, 0);

            a->r1_unqueuedsamps -= n * a->r1_outsize;
        }
        if ((a->r1_inidx += nsamples) >= a->r1_active_buffsize)
            a->r1_inidx -= a->r1_active_buffsize;
        LeaveCriticalSection(&a->csIN);
    }
    last_outbound_time = timeGetTime();
}

int obdata(int id, double* out) {
    int first, second;
    OBB a = obp.pdbuff[id];
    if (!_InterlockedAnd(&a->run, 1)) {
        return -1;
    }

    if (a->r1_outsize > (a->r1_active_buffsize - a->r1_outidx)) {
        first = a->r1_active_buffsize - a->r1_outidx;
        second = a->r1_outsize - first;
    } else {
        first = a->r1_outsize;
        second = 0;
    }
    memcpy(out, a->r1_baseptr + 2 * a->r1_outidx, first * sizeof(WDSP_COMPLEX));
    memcpy(out + 2 * first, a->r1_baseptr, second * sizeof(WDSP_COMPLEX));
    if ((a->r1_outidx += a->r1_outsize) >= a->r1_active_buffsize)
        a->r1_outidx -= a->r1_active_buffsize;

    return NOERROR;
}

void ob_main(void* pargs) {
    HANDLE hpri = prioritise_thread_max();

#pragma warning(disable : 4311)
    int id = (int)pargs;
#pragma warning(default : 4311)

    OBB a = obp.pdbuff[id];

    while (_InterlockedAnd(&a->run, 1)) {
        DWORD dw1 = timeGetTime();
        DWORD dwWait = WaitForSingleObject(a->Sem_BuffReady, 500);
        if (dwWait == WAIT_TIMEOUT) {
            continue;
        }

#ifdef DEBUG_TIMINGS
        DWORD dw2 = timeGetTime();
        volatile DWORD took = dw2 - dw1;
        if (took > 10) {
            volatile DWORD since_flagged = dw2 - a->sem_buff_flagged_when;
            printf("Took ages waiting for Sem_BuffReady %ld ms. It was flagged "
                   "%ld ms ago.\n",
                (int)took, (int)since_flagged);
        }

#endif
        EnterCriticalSection(&a->csOUT);
        LeaveCriticalSection(&a->csOUT);
        if (obdata(id, a->out) < 0) {
            break;
        }
        sendOutbound(id, a->out);
        // if (id == 0) WriteAudio(15.0, 48000, 126, a->out, 3);
    }

    if (hpri && hpri != INVALID_HANDLE_VALUE) prioritise_thread_cleanup(hpri);
}

void SetOBRingOutsize(int id, int size) {
    OBB a = obp.pcbuff[id];
    InterlockedBitTestAndReset(&a->accept, 0);
    EnterCriticalSection(&a->csIN);
    EnterCriticalSection(&a->csOUT);
    Sleep(25);
    InterlockedBitTestAndReset(&a->run, 0);
    ReleaseSemaphore(a->Sem_BuffReady, 1, 0);
    LeaveCriticalSection(&a->csOUT);
    Sleep(2);
    flush_obbuffs(id);
    a->r1_outsize = size;
    InterlockedBitTestAndSet(&a->run, 0);
    start_obthread(id);
    LeaveCriticalSection(&a->csIN);
    InterlockedBitTestAndSet(&a->accept, 0);
}
