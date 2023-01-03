// This is an independent project of an individual developer. Dear PVS-Studio,
// please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java:
// http://www.viva64.com
#pragma once
// static functions that allow the app to use Float32 inside of PortAudio.
// Blatantly stolen from pa_converters.c, with modifications. KLJ
static void Float64_To_Float32(void* destinationBuffer,
    signed int destinationStride, void* sourceBuffer, signed int sourceStride,
    unsigned int count) {
    double* src = (double*)sourceBuffer;
    float* dest = (float*)destinationBuffer;

    while (count--) {
        *dest = (float)*src;

        src += sourceStride;
        dest += destinationStride;
    }
}

static void Float32_To_Float64(void* destinationBuffer,
    signed int destinationStride, void* sourceBuffer, signed int sourceStride,
    unsigned int count) {
    float* src = (float*)sourceBuffer;
    double* dest = (double*)destinationBuffer;

    while (count--) {
        *dest = *src;

        src += sourceStride;
        dest += destinationStride;
    }
}
