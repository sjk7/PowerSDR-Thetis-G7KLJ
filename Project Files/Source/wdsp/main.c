// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
/*  main.c

This file is part of a program that implements a Software-Defined Radio.

Copyright (C) 2013 Warren Pratt, NR0V

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

#include "comm.h"

// G7KLJ: it is not a good idea to cal functions main() : renamed!
void main_fun(void* pargs) {
    HANDLE hpri = prioritise_thread_max();

    int channel = (int)pargs;
    while (_InterlockedAnd(&ch[channel].run, 1)) {
        DWORD dwWait = WaitForSingleObject(ch[channel].iob.pd->Sem_BuffReady, 400);
        if (dwWait == WAIT_TIMEOUT) {
            continue;
        }
        EnterCriticalSection(&ch[channel].csDSP);
        if (!_InterlockedAnd(&ch[channel].iob.pd->exec_bypass, 1)) {
            switch (ch[channel].type) {
                case 0: // rxa
                    dexchange(
                        channel, rxa[channel].outbuff, rxa[channel].inbuff);
                    xrxa(channel);
                    break;
                case 1: // txa
                    dexchange(
                        channel, txa[channel].outbuff, txa[channel].inbuff);
                    xtxa(channel);
                    break;
                case 31: //

                    break;
            }
        }
        LeaveCriticalSection(&ch[channel].csDSP);
    }
    
    if (hpri && hpri != INVALID_HANDLE_VALUE)
    prioritise_thread_cleanup(hpri);
    
    if (ch[channel].thread_quit_event)
    SetEvent(ch[channel].thread_quit_event);

}

void create_main(int channel) {
    switch (ch[channel].type) {
        case 0: create_rxa(channel); break;
        case 1: create_txa(channel); break;
        case 31: //

            break;
    }
}

void destroy_main(int channel) {
    switch (ch[channel].type) {
        case 0: destroy_rxa(channel); break;
        case 1: destroy_txa(channel); break;
        case 31: //

            break;
    }
}

void flush_main(int channel) {
    switch (ch[channel].type) {
        case 0: flush_rxa(channel); break;
        case 1: flush_txa(channel); break;
        case 31: break;
    }
}

void setInputSamplerate_main(int channel) {
    switch (ch[channel].type) {
        case 0: setInputSamplerate_rxa(channel); break;
        case 1: setInputSamplerate_txa(channel); break;
        case 31: //

            break;
    }
}

void setOutputSamplerate_main(int channel) {
    switch (ch[channel].type) {
        case 0: setOutputSamplerate_rxa(channel); break;
        case 1: setOutputSamplerate_txa(channel); break;
        case 31: //

            break;
    }
}

void setDSPSamplerate_main(int channel) {
    switch (ch[channel].type) {
        case 0: setDSPSamplerate_rxa(channel); break;
        case 1: setDSPSamplerate_txa(channel); break;
        case 31: //

            break;
    }
}

void setDSPBuffsize_main(int channel) {
    switch (ch[channel].type) {
        case 0: setDSPBuffsize_rxa(channel); break;
        case 1: setDSPBuffsize_txa(channel); break;
        case 31: //

            break;
    }
}
