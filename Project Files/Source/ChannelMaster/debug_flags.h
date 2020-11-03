#pragma once

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif

#include <stdio.h>  // FILE*
#include <assert.h> // assert()
#include <stddef.h> // size_t



#define STRINGX(x) #x
#define STRING(x) STRINGX(x)
#define MY_LOG(msg) __pragma(message(__FILE__ "(" STRING(__LINE__) "): " msg))

#ifndef DEBUG_TIMINGS
#if defined(DEBUG) || defined(_DEBUG) || !defined(_WIN64)
#define DEBUG_TIMINGS
MY_LOG("timings are on -- code may be slow. Luv G7KLJ xx")
#endif

#endif


#ifdef _DEBUG
#define _DEBUG_TO_FILE
#endif

static inline void dump_to_file(void* data, size_t sz) {
#ifdef _DEBUG_TO_FILE

    static FILE* fp = NULL;
    if (fp == NULL) {
        fp = fopen("test.raw", "wb");
    }

    assert(fp);
    size_t wrote = fwrite(data, 1, sz, fp);
    assert(wrote == sz);

#endif
}

static inline void dump_to_file2(void* data, size_t sz) {
#ifdef _DEBUG_TO_FILE

    static FILE* fp = NULL;
    if (fp == NULL) {
        fp = fopen("test2.raw", "wb");
    }

    assert(fp);
    size_t wrote = fwrite(data, 1, sz, fp);
    assert(wrote == sz);

#endif
}