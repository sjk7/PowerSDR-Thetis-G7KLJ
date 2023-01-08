// This is an independent project of an individual developer. Dear PVS-Studio,
// please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java:
// http://www.viva64.com
/*  ivac.c

This file is part of a program that implements a Software-Defined Radio.

Copyright (C) 2015-2019 Warren Pratt, NR0V
Copyright (C) 2015-2016 Doug Wigley, W5WC

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
#include <stdint.h>
#include "cmcomm.h"
#include "pa_win_wasapi.h"
#include "pa_win_wdmks.h"
#include "BitDepthConvert.hpp"

__declspec(align(16)) IVAC pvac[MAX_EXT_VACS];

void create_resamps(IVAC a) {
    a->MMThreadApiHandle = 0;
    // a->exclusive = 0; <-- overriding checkbox setting!
    a->INringsize = (int)(2 * a->mic_rate * a->in_latency); // FROM VAC to mic
    a->OUTringsize
        = (int)(2 * a->vac_rate * a->out_latency); // TO VAC from rx audio

    a->rmatchIN = create_rmatchV(a->vac_size, a->mic_size, a->vac_rate,
        a->mic_rate, a->INringsize); // data FROM VAC TO TX MIC INPUT
    forceRMatchVar(a->rmatchIN, a->INforce, a->INfvar);
    if (!a->iq_type)
        a->rmatchOUT = create_rmatchV(a->audio_size, a->vac_size, a->audio_rate,
            a->vac_rate, a->OUTringsize); // data FROM RADIO TO VAC
    else
        a->rmatchOUT = create_rmatchV(a->iq_size, a->vac_size, a->iq_rate,
            a->vac_rate, a->OUTringsize); // RX I-Q data going to VAC
    forceRMatchVar(a->rmatchOUT, a->OUTforce, a->OUTfvar);
    a->bitbucket = (double*)malloc0(
        getbuffsize(pcm->cmMAXInRate) * sizeof(WDSP_COMPLEX));
}

PORT void create_ivac(int id, int run,
    int iq_type, // 1 if using raw IQ samples, 0 for audio
    int stereo, // 1 for stereo, 0 otherwise
    int iq_rate, // sample rate of RX I-Q data
    int mic_rate, // sample rate of data from VAC to TX MIC input
    int audio_rate, // sample rate of data from RCVR Audio data to VAC
    int txmon_rate, // sample rate of data from TX Monitor to VAC
    int vac_rate, // VAC sample rate
    int mic_size, // buffer size for data from VAC to TX MIC input
    int iq_size, // buffer size for RCVR IQ data to VAC
    int audio_size, // buffer size for RCVR Audio data to VAC
    int txmon_size, // buffer size for TX Monitor data to VAC
    int vac_size // VAC buffer size
) {
    IVAC a = (IVAC)calloc(1, sizeof(ivac));
    if (!a) {
        printf("mem failure in ivac \n");
        exit(EXIT_FAILURE);
    }
    a->run = run;
    a->iq_type = iq_type;
    a->stereo = stereo;
    a->iq_rate = iq_rate;
    a->mic_rate = mic_rate;
    a->audio_rate = audio_rate;
    a->txmon_rate = txmon_rate;
    a->vac_rate = vac_rate;
    a->mic_size = mic_size;
    a->iq_size = iq_size;
    a->audio_size = audio_size;
    a->txmon_size = txmon_size;
    a->vac_size = vac_size;
    a->INforce = 0;
    a->INfvar = 1.0;
    a->OUTforce = 0;
    a->OUTfvar = 1.0;
    a->convbuf = 0;
    a->convbuf_size = 0;
    create_resamps(a);
    {
        int inrate[2] = {a->audio_rate, a->txmon_rate};
        a->mixer = create_aamix(-1, id, a->audio_size, a->audio_size, 2, 3, 3,
            1.0, 4096, inrate, a->audio_rate, xvac_out, 0.0, 0.0, 0.0, 0.0);
    }
    pvac[id] = a;
}

void destroy_resamps(IVAC a) {
    _aligned_free(a->bitbucket);
    destroy_rmatchV(a->rmatchOUT);
    destroy_rmatchV(a->rmatchIN);
    if (a->convbuf_size) {
        a->convbuf_size = 0;
        free(a->convbuf);
    }
}

PORT void destroy_ivac(int id) {
    IVAC a = pvac[id];
    destroy_resamps(a);
    free(a);
}

PORT void xvacIN(int id, double* in_tx, int bypass) {
    // used for MIC data to TX
    // might be a clue where to record audio from in wave recorder, since it
    // doesn't work with vac!
    IVAC a = pvac[id];
    if (a->run)
        if (!a->vac_bypass && !bypass) {
            xrmatchOUT(a->rmatchIN, in_tx);
            if (a->vac_combine_input) combinebuff(a->mic_size, in_tx, in_tx);
            scalebuff(a->mic_size, in_tx, a->vac_preamp, in_tx);
        } else
            xrmatchOUT(a->rmatchIN, a->bitbucket);
}

#ifdef DEBUG_TIMINGS
static DWORD last_times[32];
static uint32_t times_ctr = 0;
static uint32_t fails = 0;
#endif
PORT void xvacOUT(int id, int stream, double* data) {
    IVAC a = pvac[id];

#ifdef DEBUG_TIMINGS
    if (times_ctr++ > 5000) {
        DWORD since = timeGetTime() - last_times[stream];
        if (since > 10) {
            fails++;
            if (fails > 10) {

                fprintf(stderr,
                    "xvacOUT: long time between calls, for stream: %ld, took: "
                    "%ld ms.\n",
                    stream, (int)since);
            }
        }
    }
#endif

    // receiver input data (iq_type) -> stream = 0
    // receiver output data (audio)  -> stream = 1
    // transmitter output data (mon) -> stream = 2
    if (a->run) {
        if (!a->iq_type) { // call mixer to synchronize the two streams
            if (stream == 1)
                xMixAudio(a->mixer, -1, 0, data);
            else if (stream == 2)
                xMixAudio(a->mixer, -1, 1, data);
        } else if (stream == 0)
            xrmatchIN(a->rmatchOUT, data); // i-q data from RX stream
    }
#ifdef DEBUG_TIMINGS
    last_times[stream] = timeGetTime();
#endif
}

void xvac_out(int id, int nsamples,
    double* buff) { // called by the mixer with a buffer of output data
    IVAC a = pvac[id];
    xrmatchIN(a->rmatchOUT, buff); // audio data from mixer
    // if (id == 0) WriteAudio (120.0, 48000, a->audio_size, buff, 3);
}

void StreamFinishedCallback(void* userData) {

#pragma warning(disable : 4311)
    int id = (int)userData;
    IVAC a = pvac[id];
    if (a->have_set_thread_priority == 1) {
        prioritise_thread_cleanup(a->MMThreadApiHandle);
        a->have_set_thread_priority = 0;
        a->MMThreadApiHandle = 0;
    }

#pragma warning(default : 4311)
}

// KLJ
int make_ivac_thread_max_priority(IVAC a) {

    if (a->have_set_thread_priority == -1) {
        a->have_set_thread_priority = 10;
        a->MMThreadApiHandle = prioritise_thread_max();
        if (a->MMThreadApiHandle) {

            a->have_set_thread_priority = 1;
            return 1;
        } else {

            a->have_set_thread_priority = 0;
            // assert("Unable to prioritise audio thread" == 0);
            return 0;
        }
    }
    return 1;
}

static inline void size_64_bit_buffer(IVAC a, size_t sz_bytes) {
    // prepare buffer for conversion, if necessary:
    assert(sz_bytes > 0);
    size_t tmpsz = sz_bytes * 2; // oversized to avoid re-allocation.
    if (a->convbuf_size < tmpsz) {
        if (a->convbuf != 0) {
            free(a->convbuf);
            a->convbuf = 0;
        }
        a->convbuf = malloc(tmpsz * sizeof(double));
        a->convbuf_size = tmpsz;
        if (a->convbuf)
            memset(a->convbuf, 0, a->convbuf_size);
    }
}

int CallbackIVAC(const void* input, void* output, unsigned long frameCount,
    const PaStreamCallbackTimeInfo* ti, PaStreamCallbackFlags f,
    void* userData) {

    int id = (int)(ptrdiff_t)userData;
    IVAC a = pvac[id];
    const unsigned int dblSz = sizeof(double);
    const unsigned int fltSz = sizeof(float);

    if (a->have_set_thread_priority == -1) make_ivac_thread_max_priority(a);

    const size_t floatBufferSize = fltSz * frameCount * a->num_channels;
    const size_t dblBufferSize = max(a->INringsize, a->OUTringsize);
    // const size_t det = a->
    size_64_bit_buffer(a, dblBufferSize);

    float* out_ptr = (float*)output;
    float* in_ptr = (float*)input;
    if (!a->run) {
        memset(in_ptr, 0, floatBufferSize);
        memset(out_ptr, 0, floatBufferSize);
        return 0;
    }

    Float32_To_Float64(a->convbuf, 1, in_ptr, 1, frameCount * 2);
    xrmatchIN(a->rmatchIN, a->convbuf); // MIC data from VAC
    xrmatchOUT(a->rmatchOUT, a->convbuf); // audio or I-Q data to VAC
    Float64_To_Float32(out_ptr, 1, a->convbuf, 1, frameCount * 2);

    return 0;
}

PORT int StartAudioIVAC(int id) {
    IVAC a = pvac[id];
    int error = 0;
    int in_dev = Pa_HostApiDeviceIndexToDeviceIndex(
        a->host_api_index, a->input_dev_index);
    int out_dev = Pa_HostApiDeviceIndexToDeviceIndex(
        a->host_api_index, a->output_dev_index);

    a->inParam.device = in_dev;
    a->inParam.channelCount = 2;
    a->inParam.suggestedLatency = a->pa_in_latency;
    a->inParam.sampleFormat
        = paFloat32; // KLJ: Changed to support audio cards, especially loopback
                     // devices, more directly

    a->outParam.device = out_dev;
    a->outParam.channelCount = 2;
    a->outParam.suggestedLatency = a->pa_out_latency;
    a->outParam.sampleFormat = paFloat32;

    /*/
       Pa_OpenStream:

        To set desired Share Mode (Exclusive/Shared) you must supply
        PaWasapiStreamInfo with flags paWinWasapiExclusive set through member of
        PaStreamParameters::hostApiSpecificStreamInfo structure.
    /*/
    const PaHostApiInfo* hinf = Pa_GetHostApiInfo(a->host_api_index);
    PaWasapiStreamInfo w = {0};
    PaWinWDMKSInfo x = {0};
    // FIXME: There is a possibilty that w and x do not live long enough!
    // they should be in the a-> struct, probably.

    if (hinf->type == paWASAPI) {

        w.threadPriority = eThreadPriorityProAudio;
        if (a->exclusive) {
            w.flags = (paWinWasapiExclusive | paWinWasapiThreadPriority);
        }

        w.hostApiType = paWASAPI;
        w.size = sizeof(PaWasapiStreamInfo);
        w.version = 1;

        a->inParam.hostApiSpecificStreamInfo = &w;
        a->outParam.hostApiSpecificStreamInfo = &w;

    } else if (hinf->type == paWDMKS) {

        x.version = 1;
        x.hostApiType = paWDMKS;
        x.size = sizeof(PaWinWDMKSInfo);
        x.flags = paWinWDMKSOverrideFramesize;
        a->inParam.hostApiSpecificStreamInfo = &x;
        a->outParam.hostApiSpecificStreamInfo = &x;

    } else {
        a->inParam.hostApiSpecificStreamInfo = NULL;
        a->outParam.hostApiSpecificStreamInfo = NULL;
    }

#pragma warning(disable : 4312)

    error = Pa_OpenStream(&a->Stream, &a->inParam, &a->outParam, a->vac_rate,
        a->vac_size, // paFramesPerBufferUnspecified,
        0, CallbackIVAC,
        (void*)id); // pass 'id' as userData

    if (error == 0) {
        error = Pa_SetStreamFinishedCallback(a->Stream, StreamFinishedCallback);

        assert(error == 0);
        if (error == 0) {
            if (hinf->type != paWASAPI) {

                a->have_set_thread_priority
                    = -1; // go ahead and set the priority on the next call to
                          // the callback
            } else {
                // we don't do this for WASAPI, since portaudio does it for us.
                a->have_set_thread_priority = 0;
            }
        } else {
            a->have_set_thread_priority = 0;
        }
    }
#pragma warning(default : 4312)

    if (error != paNoError) return error;

    error = Pa_StartStream(a->Stream);

    if (error != paNoError) return error;

    // const PaStreamInfo* inf = Pa_GetStreamInfo(a->Stream);
    return paNoError;
}

PORT void SetIVACRBReset(int id, int reset) {
    IVAC a = pvac[id];
    // a->reset = reset;
}

PORT void StopAudioIVAC(int id) {
    IVAC a = pvac[id];
    Pa_CloseStream(a->Stream);
}

PORT void SetIVACrun(int id, int run) {
    IVAC a = pvac[id];
    a->run = run;
}

PORT void SetIVACiqType(int id, int type) {
    IVAC a = pvac[id];
    if (type != a->iq_type) {
        a->iq_type = type;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACstereo(int id, int stereo) {
    IVAC a = pvac[id];
    a->stereo = stereo;
}

PORT void SetIVACvacRate(int id, int rate) {
    IVAC a = pvac[id];
    if (rate != a->vac_rate) {
        a->vac_rate = rate;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACmicRate(int id, int rate) {
    IVAC a = pvac[id];
    if (rate != a->mic_rate) {
        a->mic_rate = rate;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACaudioRate(int id, int rate) {
    IVAC a = pvac[id];
    if (rate != a->audio_rate) {
        a->audio_rate = rate;
        destroy_aamix(a->mixer, 0);
        {
            int inrate[2] = {a->audio_rate, a->txmon_rate};
            a->mixer = create_aamix(-1, id, a->audio_size, a->audio_size, 2, 3,
                3, 1.0, 4096, inrate, a->audio_rate, xvac_out, 0.0, 0.0, 0.0,
                0.0);
        }
        destroy_resamps(a);
        create_resamps(a);
    }
}

void SetIVACtxmonRate(int id, int rate) {
    IVAC a = pvac[id];
    if (rate != a->txmon_rate) {
        a->txmon_rate = rate;
        destroy_aamix(a->mixer, 0);
        {
            int inrate[2] = {a->audio_rate, a->txmon_rate};
            a->mixer = create_aamix(-1, id, a->audio_size, a->audio_size, 2, 3,
                3, 1.0, 4096, inrate, a->audio_rate, xvac_out, 0.0, 0.0, 0.0,
                0.0);
        }
    }
}

PORT void SetIVACvacSize(int id, int size) {
    IVAC a = pvac[id];
    if (size != a->vac_size) {
        a->vac_size = size;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACmicSize(int id, int size) {
    IVAC a = pvac[id];
    if (size != a->mic_size) {
        a->mic_size = (unsigned int)size;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACiqSizeAndRate(int id, int size, int rate) {
    IVAC a = pvac[id];
    if (size != a->iq_size || rate != a->iq_rate) {
        a->iq_size = size;
        a->iq_rate = rate;
        if (a->iq_type) {
            destroy_resamps(a);
            create_resamps(a);
        }
    }
}

PORT void SetIVACaudioSize(int id, int size) {
    IVAC a = pvac[id];
    a->audio_size = (unsigned int)size;
    destroy_aamix(a->mixer, 0);
    {
        int inrate[2] = {a->audio_rate, a->txmon_rate};
        a->mixer = create_aamix(-1, id, a->audio_size, a->audio_size, 2, 3, 3,
            1.0, 4096, inrate, a->audio_rate, xvac_out, 0.0, 0.0, 0.0, 0.0);
    }
    destroy_resamps(a);
    create_resamps(a);
}

void SetIVACtxmonSize(int id, int size) {
    IVAC a = pvac[id];
    a->txmon_size = (unsigned int)size;
}

PORT void SetIVAChostAPIindex(int id, int index) {
    IVAC a = pvac[id];
    a->host_api_index = index;
}

PORT void SetIVACinputDEVindex(int id, int index) {
    IVAC a = pvac[id];
    a->input_dev_index = index;
}

PORT void SetIVACoutputDEVindex(int id, int index) {
    IVAC a = pvac[id];
    a->output_dev_index = index;
}

PORT void SetIVACnumChannels(int id, int n) {
    IVAC a = pvac[id];
    a->num_channels = n;
}

PORT void SetIVACInLatency(int id, double lat, int reset) {
    IVAC a = pvac[id];

    if (a->in_latency != lat) {
        a->in_latency = lat;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACOutLatency(int id, double lat, int reset) {
    IVAC a = pvac[id];

    if (a->out_latency != lat) {
        a->out_latency = lat;
        destroy_resamps(a);
        create_resamps(a);
    }
}

PORT void SetIVACPAInLatency(int id, double lat, int reset) {
    IVAC a = pvac[id];

    if (a->pa_in_latency != lat) {
        a->pa_in_latency = lat;
    }
}

PORT void SetIVACExclusive(int id, int excl) {
    IVAC a = pvac[id];
    a->exclusive = excl;
}

PORT int GetIVACExclusive(int id) {
    IVAC a = pvac[id];
    return a->exclusive;
}

PORT void SetIVACPAOutLatency(int id, double lat, int reset) {
    IVAC a = pvac[id];

    if (a->pa_out_latency != lat) {
        a->pa_out_latency = lat;
    }
}

PORT void SetIVACvox(int id, int vox) {
    IVAC a = pvac[id];
    a->vox = vox;
}

PORT void SetIVACmox(int id, int mox) {
    IVAC a = pvac[id];

    a->mox = mox;

    if (!a->mox) {
        SetAAudioMixWhat(a->mixer, 0, 1, 0);
        SetAAudioMixWhat(a->mixer, 0, 0, 1);
    } else if (a->mon) {
        SetAAudioMixWhat(a->mixer, 0, 0, 0);
        SetAAudioMixWhat(a->mixer, 0, 1, 1);
    } else {
        SetAAudioMixWhat(a->mixer, 0, 0, 0);
        SetAAudioMixWhat(a->mixer, 0, 1, 0);
    }
}

PORT void SetIVACmon(int id, int mon) {
    IVAC a = pvac[id];

    a->mon = mon;

    if (!a->mox) {
        SetAAudioMixWhat(a->mixer, 0, 1, 0);
        SetAAudioMixWhat(a->mixer, 0, 0, 1);
    } else if (mon) {
        SetAAudioMixWhat(a->mixer, 0, 0, 0);
        SetAAudioMixWhat(a->mixer, 0, 1, 1);
    } else {
        SetAAudioMixWhat(a->mixer, 0, 0, 0);
        SetAAudioMixWhat(a->mixer, 0, 1, 0);
    }
}

PORT void SetIVACMonVolume(int id, double vol) {

    if (id == -1) {
        // overall monitor gain
        IVAC pa = pvac[0];
        AAMIX a = (AAMIX)pa->mixer;
        a->volume = vol;
        for (int i = 0; i < 32; ++i) {
            a->tvol[i] = vol * a->vol[i];
        }
        return;
    }
    // tx mon only:
    IVAC pa = pvac[id];
    assert(pa->mixer);
    AAMIX a = (AAMIX)pa->mixer;

    EnterCriticalSection(&a->cs_out);
    const int mon_index = 1;
    a->tvol[mon_index] = vol * a->vol[mon_index];
    LeaveCriticalSection(&a->cs_out);
}

PORT void SetIVACpreamp(int id, double preamp) {
    IVAC a = pvac[id];
    a->vac_preamp = preamp;
}

PORT void SetIVACrxscale(int id, double scale) {
    IVAC a = pvac[id];
    a->vac_rx_scale = scale;
    SetAAudioMixVolume(a->mixer, 0, a->vac_rx_scale);
}

PORT void SetIVACbypass(int id, int bypass) {
    IVAC a = pvac[id];
    a->vac_bypass = bypass;
}

PORT void SetIVACcombine(int id, int combine) {
    IVAC a = pvac[id];
    a->vac_combine_input = combine;
}

void combinebuff(int n, double* a, double* combined) {
    int i;
    for (i = 0; i < 2 * n; i += 2)
        combined[i] = combined[i + 1] = a[i] + a[i + 1];
}

void scalebuff(int size, double* in, double scale, double* out) {
    int i;
    for (i = 0; i < 2 * size; i++) out[i] = scale * in[i];
}

PORT void getIVACdiags(int id, int type, int* underflows, int* overflows,
    double* var, int* ringsize) {
    // type:  0 - From VAC; 1 - To VAC
    void* a;
    if (type == 0)
        a = pvac[id]->rmatchOUT;
    else
        a = pvac[id]->rmatchIN;
    getRMatchDiags(a, underflows, overflows, var, ringsize);
}

PORT void forceIVACvar(int id, int type, int force, double fvar) {
    // type:  0 - From VAC; 1 - To VAC
    IVAC b = pvac[id];
    void* a;
    if (type == 0) {
        a = b->rmatchOUT;
        b->OUTforce = force;
        b->OUTfvar = fvar;
    } else {
        a = b->rmatchIN;
        b->INforce = force;
        b->INfvar = fvar;
    }
    forceRMatchVar(a, force, fvar);
}
PORT void resetIVACdiags(int id, int type) {
    // type:  0 - From VAC; 1 - To VAC
    void* a;
    if (type == 0)
        a = pvac[id]->rmatchOUT;
    else
        a = pvac[id]->rmatchIN;
    resetRMatchDiags(a);
}
