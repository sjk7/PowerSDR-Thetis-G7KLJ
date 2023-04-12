//=================================================================
// audio.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2009  FlexRadio Systems
// Copyright (C) 2010-2020  Doug Wigley
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// You may contact us via email at: sales@flex-radio.com.
// Paper mail may be sent to:
//    FlexRadio Systems
//    8900 Marybank Dr.
//    Austin, TX 78750
//    USA
//=================================================================

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;
using Thetis.Midi2Cat;
// using ProjectCeilidh.PortAudio;
// using ProjectCeilidh.PortAudio.Native;
// using static ProjectCeilidh.PortAudio.Native.PortAudio;

namespace Thetis
{
    public class Audio
    {
        #region Thetis Specific Variables

        // ======================================================
        // Thetis Specific Variables
        // ======================================================

        public enum AudioState
        {
            DTTSP = 0,
            CW,
        }

        public enum SignalSource
        {
            RADIO,
            SINE,
            SINE_TWO_TONE,
            SINE_LEFT_ONLY,
            SINE_RIGHT_ONLY,
            NOISE,
            TRIANGLE,
            SAWTOOTH,
            PULSE,
            SILENCE,
        }

        // unsafe private static PortAudioForThetis.PaStreamCallback PAcallbackport
        // = PACallbackPort;

        // public static int callback_return;

        private static bool rx2_auto_mute_tx = true;
        public static bool RX2AutoMuteTX
        {
            get { return rx2_auto_mute_tx; }
            set { rx2_auto_mute_tx = value; }
        }

        private static bool rx1_blank_display_tx = false;
        public static bool RX1BlankDisplayTX
        {
            get { return rx1_blank_display_tx; }
            set { rx1_blank_display_tx = value; }
        }

        private static bool rx2_blank_display_tx = false;
        public static bool RX2BlankDisplayTX
        {
            get { return rx2_blank_display_tx; }
            set { rx2_blank_display_tx = value; }
        }

        private static double source_scale = 1.0;
        public static double SourceScale
        {
            get { return source_scale; }
            set { source_scale = value; }
        }

        private static SignalSource tx_input_signal = SignalSource.RADIO;
        public static SignalSource TXInputSignal
        {
            get { return tx_input_signal; }
            set { tx_input_signal = value; }
        }

        private static SignalSource tx_output_signal = SignalSource.RADIO;
        public static SignalSource TXOutputSignal
        {
            get { return tx_output_signal; }
            set { tx_output_signal = value; }
        }

        private static bool record_rx_preprocessed = false;
        public static bool RecordRXPreProcessed
        {
            get { return record_rx_preprocessed; }
            set
            {
                record_rx_preprocessed = value;
                WaveThing.wrecorder[0].RxPre = value;
                WaveThing.wrecorder[1].RxPre = value;
            }
        }

        private static bool record_tx_preprocessed = true;
        public static bool RecordTXPreProcessed
        {
            get { return record_tx_preprocessed; }
            set
            {
                record_tx_preprocessed = value;
                WaveThing.wrecorder[0].TxPre = value;
                WaveThing.wrecorder[1].TxPre = value;
            }
        }

        private static short bit_depth = 32;
        public static short BitDepth
        {
            get { return bit_depth; }
            set { bit_depth = value; }
        }

        private static short format_tag = 3;
        public static short FormatTag
        {
            get { return format_tag; }
            set { format_tag = value; }
        }

        private static bool vox_enabled = false;
        public static bool VOXEnabled
        {
            get { return vox_enabled; }
            set
            {
                vox_enabled = value;
                cmaster.CMSetTXAVoxRun(0);
                if (vox_enabled && vfob_tx)
                {
                    ivac.SetIVACvox(0, 0);
                    ivac.SetIVACvox(1, 1);
                }
                if (vox_enabled && !vfob_tx)
                {
                    ivac.SetIVACvox(0, 1);
                    ivac.SetIVACvox(1, 0);
                }
                if (!vox_enabled)
                {
                    ivac.SetIVACvox(0, 0);
                    ivac.SetIVACvox(1, 0);
                }
            }
        }

        private static float vox_gain = 1.0f;
        public static float VOXGain
        {
            get { return vox_gain; }
            set { vox_gain = value; }
        }

        private static double high_swr_scale = 1.0;
        public static double HighSWRScale
        {
            get { return high_swr_scale; }
            set
            {
                high_swr_scale = value;
                cmaster.CMSetTXOutputLevel();
            }
        }

        private static double mic_preamp = 1.0;
        public static double MicPreamp
        {
            get { return mic_preamp; }
            set
            {
                mic_preamp = value;
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
            }
        }

        private static double wave_preamp = 1.0;
        public static double WavePreamp
        {
            get { return wave_preamp; }
            set
            {
                wave_preamp = value;
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
            }
        }

        private static double monitor_volume = 0.0;
        public static double MonitorVolume
        {
            get { return monitor_volume; }
            set
            {
                monitor_volume = value;
                cmaster.CMSetAudioVolume(value);
                // G7KLJ bugfix: tx mon volume was not working on VAC!
                if (console.VACEnabled)
                {
                    if (console.MOX)
                    {
                        ivac.SetIVACMonVolume(0, value);
                    }
                    else
                    {
                        // overall gain:
                        ivac.SetIVACMonVolume(-1, value);
                    }
                }
            }
        }

        private static double radio_volume = 0.0;
        public static double RadioVolume
        {
            get { return radio_volume; }
            set
            {
                radio_volume = value;
                NetworkIO.SetOutputPower((float)(value * 1.02));
                cmaster.CMSetTXOutputLevel();
            }
        }

        private static AudioState current_audio_state1 = AudioState.DTTSP;
        public static AudioState CurrentAudioState1
        {
            get { return current_audio_state1; }
            set { current_audio_state1 = value; }
        }

        private static bool rx2_enabled;

        public static bool RX2Enabled
        {
            get { return rx2_enabled; }
            set { rx2_enabled = value; }
        }

        private static bool wave_playback = false;
        public static bool WavePlayback
        {
            get { return wave_playback; }
            set
            {
                wave_playback = value;
                cmaster.CMSetSRXWavePlayRun(0);
                cmaster.CMSetSRXWavePlayRun(1);
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
            }
        }

        private static bool wave_record;
        public static bool WaveRecord
        {
            get { return wave_record; }
            set
            {
                wave_record = value;
                cmaster.CMSetSRXWaveRecordRun(0);
                cmaster.CMSetSRXWaveRecordRun(1);
            }
        }

        public static Console console;
        public static float[] phase_buf_l;
        public static float[] phase_buf_r;
        public static bool phase;
        public static bool scope;

        public static bool two_tone;
        public static bool high_pwr_am;
        public static bool testing;

        private static bool vac_combine_input = false;
        public static bool VACCombineInput
        {
            get { return vac_combine_input; }
            set
            {
                vac_combine_input = value;
                ivac.SetIVACcombine(0, Convert.ToInt32(value));
            }
        }

        private static bool vac2_combine_input = false;
        public static bool VAC2CombineInput
        {
            get { return vac2_combine_input; }
            set
            {
                vac2_combine_input = value;
                ivac.SetIVACcombine(1, Convert.ToInt32(value));
            }
        }

        #endregion

        #region Local Copies of External Properties

        private static bool mox = false;
        public static bool MOX
        {
            get { return mox; }
            set
            {
                mox = value;
                if (mox && vfob_tx)
                {
                    ivac.SetIVACmox(0, 0);
                    ivac.SetIVACmox(1, 1);
                }
                if (mox && !vfob_tx)
                {
                    ivac.SetIVACmox(0, 1);
                    ivac.SetIVACmox(1, 0);
                }
                if (!mox)
                {
                    ivac.SetIVACmox(0, 0);
                    ivac.SetIVACmox(1, 0);
                }
            }
        }

        private static bool mon;
        public static bool MON
        {
            set
            {
                mon = value;
                if (mon && vfob_tx)
                {
                    ivac.SetIVACmon(0, 0);
                    ivac.SetIVACmon(1, 1);
                }
                if (mon && !vfob_tx)
                {
                    ivac.SetIVACmon(0, 1);
                    ivac.SetIVACmon(1, 0);
                }
                if (!mon)
                {
                    ivac.SetIVACmon(0, 0);
                    ivac.SetIVACmon(1, 0);
                }
                unsafe
                {
                    cmaster.SetAAudioMixVol((void*)0, 0, WDSP.id(1, 0), 0.5);
                    cmaster.SetAAudioMixWhat((void*)0, 0, WDSP.id(1, 0), value);
                }
            }
            get { return mon; }
        }

        private static bool full_duplex = false;
        public static bool FullDuplex
        {
            set { full_duplex = value; }
        }

        private static bool vfob_tx = false;
        public static bool VFOBTX
        {
            set { vfob_tx = value; }
        }

        private static bool antivox_source_VAC = false;
        public static bool AntiVOXSourceVAC
        {
            get { return antivox_source_VAC; }
            set
            {
                antivox_source_VAC = value;
                cmaster.CMSetAntiVoxSourceWhat();
            }
        }

        private static bool vac_enabled = false;
        public static bool VACEnabled
        {
            set
            {
                vac_enabled = value;
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
                cmaster.CMSetAntiVoxSourceWhat();
                if (console.PowerOn)
                {
                    EnableVAC1(value);
                }
            }
            get { return vac_enabled; }
        }

        private static bool vac2_enabled = false;
        public static bool VAC2Enabled
        {
            set
            {
                vac2_enabled = value;
                cmaster.CMSetAntiVoxSourceWhat();
                if (console.PowerOn)
                {
                    EnableVAC2(value);
                }
            }
            get { return vac2_enabled; }
        }

        private static bool vac1_latency_manual = false;
        public static bool VAC1LatencyManual
        {
            set { vac1_latency_manual = value; }
            get { return vac1_latency_manual; }
        }

        private static bool vac1_latency_manual_out = false;
        public static bool VAC1LatencyManualOut
        {
            set { vac1_latency_manual_out = value; }
            get { return vac1_latency_manual_out; }
        }

        private static bool vac1_latency_pa_in_manual = false;
        public static bool VAC1LatencyPAInManual
        {
            set { vac1_latency_pa_in_manual = value; }
            get { return vac1_latency_pa_in_manual; }
        }

        private static bool vac1_latency_pa_out_manual = false;
        public static bool VAC1LatencyPAOutManual
        {
            set { vac1_latency_pa_out_manual = value; }
            get { return vac1_latency_pa_out_manual; }
        }

        private static bool vac2_latency_manual = false;
        public static bool VAC2LatencyManual
        {
            set { vac2_latency_manual = value; }
            get { return vac2_latency_manual; }
        }

        private static bool vac2_latency_out_manual = false;
        public static bool VAC2LatencyOutManual
        {
            set { vac2_latency_out_manual = value; }
            get { return vac2_latency_out_manual; }
        }

        private static bool vac2_latency_pa_in_manual = false;
        public static bool VAC2LatencyPAInManual
        {
            set { vac2_latency_pa_in_manual = value; }
            get { return vac2_latency_pa_in_manual; }
        }

        private static bool vac2_latency_pa_out_manual = false;
        public static bool VAC2LatencyPAOutManual
        {
            set { vac2_latency_pa_out_manual = value; }
            get { return vac2_latency_pa_out_manual; }
        }

        private static bool vac_bypass = false;
        public static bool VACBypass
        {
            get { return vac_bypass; }
            set
            {
                vac_bypass = value;
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
                ivac.SetIVACbypass(0, Convert.ToInt32(value));
                ivac.SetIVACbypass(1, Convert.ToInt32(value));
            }
        }

        private static bool vac_rb_reset = false;
        public static bool VACRBReset
        {
            set
            {
                vac_rb_reset = value;
                ivac.SetIVACRBReset(0, Convert.ToInt32(value));
            }
            get { return vac_rb_reset; }
        }

        private static bool vac2_rb_reset = false;
        public static bool VAC2RBReset
        {
            set
            {
                vac2_rb_reset = value;
                ivac.SetIVACRBReset(1, Convert.ToInt32(value));
            }
            get { return vac2_rb_reset; }
        }

        private static double vac_preamp = 1.0;
        public static double VACPreamp
        {
            get { return vac_preamp; }
            set
            {
                vac_preamp = value;
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
                ivac.SetIVACpreamp(0, value);
            }
        }

        private static double vac2_tx_scale = 1.0;
        public static double VAC2TXScale
        {
            get { return vac2_tx_scale; }
            set
            {
                vac2_tx_scale = value;
                ivac.SetIVACpreamp(1, value);
            }
        }

        private static double vac_rx_scale = 1.0;
        public static double VACRXScale
        {
            get { return vac_rx_scale; }
            set
            {
                vac_rx_scale = value;
                ivac.SetIVACrxscale(0, value);
            }
        }

        private static double vac2_rx_scale = 1.0;
        public static double VAC2RXScale
        {
            get { return vac2_rx_scale; }
            set
            {
                vac2_rx_scale = value;
                ivac.SetIVACrxscale(1, value);
            }
        }

        private static DSPMode tx_dsp_mode = DSPMode.LSB;
        public static DSPMode TXDSPMode
        {
            get { return tx_dsp_mode; }
            set
            {
                tx_dsp_mode = value;
                cmaster.CMSetTXAVoxRun(0);
                cmaster.CMSetTXAPanelGain1(WDSP.id(1, 0));
            }
        }

        private static int sample_rate1 = 48000;
        public static int SampleRate1
        {
            get { return sample_rate1; }
            set
            {
                sample_rate1 = value;
                SetOutCount();
                // set input sample rate for receivers
                cmaster.SetXcmInrate(0, value);
                // cmaster.SetXcmInrate(3, value);
                // cmaster.SetXcmInrate(4, value);
            }
        }

        private static int sample_rate_rx2 = 48000;
        public static int SampleRateRX2
        {
            get { return sample_rate_rx2; }
            set
            {
                sample_rate_rx2 = value;
                cmaster.SetXcmInrate(1, value);
                SetOutCountRX2();
            }
        }

        private static int sample_rate_tx = 48000;
        public static int SampleRateTX
        {
            get { return sample_rate_tx; }
            set
            {
                sample_rate_tx = value;
                SetOutCountTX();
            }
        }

        private static int sample_rate2 = 48000;
        public static int SampleRate2
        {
            get { return sample_rate2; }
            set
            {
                sample_rate2 = value;
                ivac.SetIVACvacRate(0, value);
            }
        }

        private static int sample_rate3 = 48000;
        public static int SampleRate3
        {
            get { return sample_rate3; }
            set
            {
                sample_rate3 = value;
                ivac.SetIVACvacRate(1, value);
            }
        }

        private static int block_size1 = 1024;
        public static int BlockSize
        {
            get { return block_size1; }
            set
            {
                block_size1 = value;
                SetOutCount();
            }
        }

        private static int block_size_rx2 = 1024;
        public static int BlockSizeRX2
        {
            get { return block_size_rx2; }
            set
            {
                block_size_rx2 = value;
                SetOutCountRX2();
            }
        }

        private static int block_size_vac = 1024;
        public static int BlockSizeVAC
        {
            get { return block_size_vac; }
            set
            {
                block_size_vac = value;
                ivac.SetIVACvacSize(0, value);
            }
        }

        private static int block_size_vac2 = 1024;
        public static int BlockSizeVAC2
        {
            get { return block_size_vac2; }
            set
            {
                block_size_vac2 = value;
                ivac.SetIVACvacSize(1, value);
            }
        }

        private static bool vac_stereo = false;
        public static bool VACStereo
        {
            get { return vac_stereo; }
            set
            {
                vac_stereo = value;
                ivac.SetIVACstereo(0, Convert.ToInt32(value));
            }
        }

        private static bool vac2_stereo = false;
        public static bool VAC2Stereo
        {
            set
            {
                vac2_stereo = value;
                ivac.SetIVACstereo(1, Convert.ToInt32(value));
            }
        }

        private static bool vac_output_iq = false;
        public static bool VACOutputIQ
        {
            get { return vac_output_iq; }
            set
            {
                vac_output_iq = value;
                ivac.SetIVACiqType(0, Convert.ToInt32(value));
            }
        }

        private static bool vac2_output_iq = false;
        public static bool VAC2OutputIQ
        {
            set
            {
                vac2_output_iq = value;
                ivac.SetIVACiqType(1, Convert.ToInt32(value));
            }
        }

        private static bool vac_output_rx2 = false;
        public static bool VACOutputRX2
        {
            set { vac_output_rx2 = value; }
        }

        private static bool vac_correct_iq = true;
        public static bool VACCorrectIQ
        {
            set { vac_correct_iq = value; }
        }

        private static bool vac2_correct_iq = true;
        public static bool VAC2CorrectIQ
        {
            set { vac2_correct_iq = value; }
        }

        private static bool vox_active = false;
        public static bool VOXActive
        {
            get { return vox_active; }
            set { vox_active = value; }
        }

        private static int host2 = 0;
        public static int Host2
        {
            get { return host2; }
            set { host2 = value; }
        }

        private static int host3 = 0;
        public static int Host3
        {
            get { return host3; }
            set { host3 = value; }
        }

        private static int input_dev2 = 0;
        public static int Input2
        {
            get { return input_dev2; }
            set { input_dev2 = value; }
        }

        private static int input_dev3 = 0;
        public static int Input3
        {
            get { return input_dev3; }
            set { input_dev3 = value; }
        }

        private static int output_dev2 = 0;
        public static int Output2
        {
            get { return output_dev2; }
            set { output_dev2 = value; }
        }

        private static int output_dev3 = 0;
        public static int Output3
        {
            get { return output_dev3; }
            set { output_dev3 = value; }
        }

        private static int latency2 = 120;
        public static int Latency2
        {
            set { latency2 = value; }
        }

        private static int latency2_out = 120;
        public static int Latency2_Out
        {
            set { latency2_out = value; }
        }

        private static int latency_pa_in = 120;
        public static int LatencyPAIn
        {
            set { latency_pa_in = value; }
        }

        private static int latency_pa_out = 120;
        public static int LatencyPAOut
        {
            set { latency_pa_out = value; }
        }

        private static int vac2_latency_out = 120;
        public static int VAC2LatencyOut
        {
            set { vac2_latency_out = value; }
        }

        private static int vac2_latency_pa_in = 120;
        public static int VAC2LatencyPAIn
        {
            set { vac2_latency_pa_in = value; }
        }

        private static int vac2_latency_pa_out = 120;
        public static int VAC2LatencyPAOut
        {
            set { vac2_latency_pa_out = value; }
        }

        private static int latency3 = 120;
        public static int Latency3
        {
            set { latency3 = value; }
        }

        private static bool mute_rx1 = false;
        public static bool MuteRX1
        {
            get { return mute_rx1; }
            set
            {
                mute_rx1 = value;
                unsafe
                {
                    cmaster.SetAAudioMixWhat((void*)0, 0, 0, !mute_rx1);
                    cmaster.SetAAudioMixWhat((void*)0, 0, 1, !mute_rx1);
                    if (console.VACEnabled)
                    {
                        if (value)
                        {
                            ivac.SetIVACMonVolume(-1, 0);
                        }
                        else
                        {

                            ivac.SetIVACMonVolume(-1, MonitorVolume);
                        }
                    }
                }
            }
        }

        private static bool mute_rx2 = false;
        public static bool MuteRX2
        {
            get { return mute_rx2; }
            set
            {
                mute_rx2 = value;
                unsafe
                {
                    cmaster.SetAAudioMixWhat((void*)0, 0, 2, !mute_rx2);
                }
            }
        }

        private static int out_rate = 48000;
        public static int OutRate
        {
            get { return out_rate; }
            set
            {
                out_rate = value;
                SetOutCount();
            }
        }

        private static int out_rate_rx2 = 48000;
        public static int OutRateRX2
        {
            get { return out_rate_rx2; }
            set
            {
                out_rate_rx2 = value;
                SetOutCountRX2();
            }
        }

        private static int out_rate_tx = 48000;
        public static int OutRateTX
        {
            get { return out_rate_tx; }
            set
            {
                out_rate_tx = value;
                SetOutCountTX();
            }
        }

        private static void SetOutCount()
        {
            if (out_rate >= sample_rate1)
                OutCount = block_size1 * (out_rate / sample_rate1);
            else
                OutCount = block_size1 / (sample_rate1 / out_rate);
        }

        private static void SetOutCountRX2()
        {
            if (out_rate_rx2 >= sample_rate_rx2)
                OutCountRX2 = block_size_rx2 * (out_rate_rx2 / sample_rate_rx2);
            else
                OutCountRX2 = block_size_rx2 / (sample_rate_rx2 / out_rate_rx2);
        }

        private static void SetOutCountTX()
        {
            if (out_rate_tx >= sample_rate_tx)
                OutCountTX = block_size1 * (out_rate_tx / sample_rate_tx);
            else
                OutCountTX = block_size1 / (sample_rate_tx / out_rate_tx);
        }

        private static int out_count = 1024;
        public static int OutCount
        {
            get { return out_count; }
            set { out_count = value; }
        }

        private static int out_count_rx2 = 1024;
        public static int OutCountRX2
        {
            get { return out_count_rx2; }
            set { out_count_rx2 = value; }
        }

        private static int out_count_tx = 1024;
        public static int OutCountTX
        {
            get { return out_count_tx; }
            set { out_count_tx = value; }
        }

        #endregion

        #region Callback Routines

        #endregion

        #region Misc Routines

        public static ArrayList GetPAHosts() // returns a text list of driver types
        {
            var a = new ArrayList();
            for (int i = 0; i < PortAudioForThetis.PA_GetHostApiCount(); i++)
            {
                PortAudioForThetis.PaHostApiInfo info
                    = PortAudioForThetis.PA_GetHostApiInfo(i);
                a.Add(info.name);
            }
            a.Add("HPSDR (USB/UDP)");
            return a;
        }

        public static ArrayList GetPAInputDevices(int hostIndex)
        {
            var a = new ArrayList();

            if (hostIndex >= PortAudioForThetis.PA_GetHostApiCount())
            {
                a.Add(new PADeviceInfo("HPSDR (PCM A/D)", 0));
                return a;
            }

            PortAudioForThetis.PaHostApiInfo hostInfo
                = PortAudioForThetis.PA_GetHostApiInfo(hostIndex);
            for (int i = 0; i < hostInfo.deviceCount; i++)
            {
                int devIndex
                    = PortAudioForThetis.PA_HostApiDeviceIndexToDeviceIndex(
                        hostIndex, i);
                PortAudioForThetis.PaDeviceInfo devInfo
                    = PortAudioForThetis.PA_GetDeviceInfo(devIndex);
                if (devInfo.maxInputChannels > 0)
                {
                    string name = devInfo.name;
                    int index = name.IndexOf("- ");
                    if (index > 0)
                    {
                        char c = name[index
                            - 1]; // make sure this is what we're looking for
                        if (c >= '0' && c <= '9') // it is... remove index
                        {
                            int x = name.IndexOf("(");
                            name = devInfo.name.Substring(0,
                                x + 1); // grab first part of string including "("
                            name += devInfo.name.Substring(index + 2,
                                devInfo.name.Length - index
                                    - 2); // add end of string;
                        }
                    }
                    a.Add(new PADeviceInfo(name, i) /* + " - " + devIndex*/);
                }
            }
            return a;
        }

        public static bool CheckPAInputDevices(int hostIndex, string name)
        {
            PortAudioForThetis.PaHostApiInfo hostInfo
                = PortAudioForThetis.PA_GetHostApiInfo(hostIndex);
            for (int i = 0; i < hostInfo.deviceCount; i++)
            {
                int devIndex
                    = PortAudioForThetis.PA_HostApiDeviceIndexToDeviceIndex(
                        hostIndex, i);
                PortAudioForThetis.PaDeviceInfo devInfo
                    = PortAudioForThetis.PA_GetDeviceInfo(devIndex);
                if (devInfo.maxInputChannels > 0 && devInfo.name.Contains(name))
                    return true;
            }
            return false;
        }

        public static ArrayList GetPAOutputDevices(int hostIndex)
        {
            var a = new ArrayList();

            if (hostIndex >= PortAudioForThetis.PA_GetHostApiCount())
            {
                a.Add(new PADeviceInfo("HPSDR (PWM D/A)", 0));
                return a;
            }

            PortAudioForThetis.PaHostApiInfo hostInfo
                = PortAudioForThetis.PA_GetHostApiInfo(hostIndex);
            for (int i = 0; i < hostInfo.deviceCount; i++)
            {
                int devIndex
                    = PortAudioForThetis.PA_HostApiDeviceIndexToDeviceIndex(
                        hostIndex, i);
                PortAudioForThetis.PaDeviceInfo devInfo
                    = PortAudioForThetis.PA_GetDeviceInfo(devIndex);
                if (devInfo.maxOutputChannels > 0)
                {
                    string name = devInfo.name;
                    int index = name.IndexOf("- "); // find case for things like
                                                    // "Microphone (2- FLEX-1500)"
                    if (index > 0)
                    {
                        char c = name[index
                            - 1]; // make sure this is what we're looking for
                        if (c >= '0' && c <= '9') // it is... remove index
                        {
                            int x = name.IndexOf("(");
                            name = devInfo.name.Substring(0,
                                x + 1); // grab first part of string including "("
                            name += devInfo.name.Substring(index + 2,
                                devInfo.name.Length - index
                                    - 2); // add end of string;
                        }
                    }
                    a.Add(new PADeviceInfo(name, i) /* + " - " + devIndex*/);
                }
            }
            return a;
        }

        public static bool CheckPAOutputDevices(int hostIndex, string name)
        {
            PortAudioForThetis.PaHostApiInfo hostInfo
                = PortAudioForThetis.PA_GetHostApiInfo(hostIndex);
            for (int i = 0; i < hostInfo.deviceCount; i++)
            {
                int devIndex
                    = PortAudioForThetis.PA_HostApiDeviceIndexToDeviceIndex(
                        hostIndex, i);
                PortAudioForThetis.PaDeviceInfo devInfo
                    = PortAudioForThetis.PA_GetDeviceInfo(devIndex);
                if (devInfo.maxOutputChannels > 0 && devInfo.name.Contains(name))
                    return true;
            }
            return false;
        }

        // public static ArrayList GetPAHosts() // returns a text list of driver
        // types
        //{
        //     var a = new ArrayList();

        //    foreach (var api in PortAudioHostApi.SupportedHostApis)
        //    {
        //        a.Add(api.Name);
        //    }
        //    a.Add("HPSDR (USB/UDP)");
        //    return a;
        //}

        // public static ArrayList GetPAInputDevices(int hostIndex)
        //{
        //     var a = new ArrayList();

        //    if (hostIndex >= PortAudioHostApi.SupportedHostApis.Count())
        //    {
        //        a.Add(new PADeviceInfo("HPSDR (PCM A/D)", 0));
        //        return a;
        //    }

        //    var hst = PortAudioHostApi.SupportedHostApis.ElementAt(hostIndex);
        //    int idx = 0;
        //        foreach (var dev in hst.Devices)
        //        {
        //            if (dev.MaxInputChannels > 0)
        //            {
        //                string name = dev.Name;
        //                int index = name.IndexOf("- ");
        //                if (index > 0)
        //                {
        //                    char c = name[index - 1]; // make sure this is what
        //                    we're looking for if (c >= '0' && c <= '9') // it
        //                    is... remove index
        //                    {
        //                        int x = name.IndexOf("(");
        //                        name = dev.Name.Substring(0, x + 1); // grab first
        //                        part of string including "(" name +=
        //                        dev.Name.Substring(index + 2, dev.Name.Length -
        //                        index - 2); // add end of string;
        //                    }
        //                }
        //                a.Add(new PADeviceInfo(name, idx));
        //            idx++;
        //            }
        //        }

        //    return a;
        //}

        // public static ArrayList GetPAOutputDevices(int hostIndex)
        //{
        //     var a = new ArrayList();

        //    if (hostIndex >= PortAudioHostApi.SupportedHostApis.Count())
        //    {
        //        a.Add(new PADeviceInfo("HPSDR (PWM D/A)", 0));
        //        return a;
        //    }

        //    var hst = PortAudioHostApi.SupportedHostApis.ElementAt(hostIndex);
        //    int idx = 0;
        //        foreach (var dev in hst.Devices)
        //        {
        //            if (dev.MaxOutputChannels > 0)
        //            {
        //                string name = dev.Name;
        //                int index = name.IndexOf("- ");
        //                if (index > 0)
        //                {
        //                    char c = name[index - 1]; // make sure this is what
        //                    we're looking for if (c >= '0' && c <= '9') // it
        //                    is... remove index
        //                    {
        //                        int x = name.IndexOf("(");
        //                        name = dev.Name.Substring(0, x + 1); // grab first
        //                        part of string including "(" name +=
        //                        dev.Name.Substring(index + 2, dev.Name.Length -
        //                        index - 2); // add end of string;
        //                    }
        //                }
        //                a.Add(new PADeviceInfo(name, idx));
        //                idx++;
        //            }
        //        }

        //    return a;
        //}

        public static void EnableVAC1Exclusive(bool ex)
        {
            ivac.SetIVACExclusive(0, ex ? 1 : 0);
        }

        public static bool GetVAC1Exclusive()
        {
            return ivac.GetIVACExclusive(0) != 0;
        }

        public struct VACStatus
        {
            public bool state;
            public String status;
        };

        public static VACStatus[] Status = new VACStatus[2];
        public static void EnableVAC1(bool enable)
        {

            string pa_msg = "";

            if (enable) unsafe
                {
                    Status[0].state = true;
                    Status[0].status = "";
                    int num_chan = 1;
                    int sample_rate = sample_rate2;
                    int block_size = block_size_vac;
                    var indevinfo = PortAudioForThetis.PA_GetDeviceInfo(input_dev2);
                    var outdevinfo
                        = PortAudioForThetis.PA_GetDeviceInfo(output_dev2);

                    double in_latency = vac1_latency_manual
                    ? latency2 / 1000.0
                    : indevinfo.defaultLowInputLatency;
                    double out_latency = vac1_latency_manual_out
                        ? latency2_out / 1000.0
                        : outdevinfo.defaultLowOutputLatency;
                    double pa_in_latency = vac1_latency_pa_in_manual
                        ? latency_pa_in / 1000.0
                        : indevinfo.defaultLowInputLatency;
                    double pa_out_latency = vac1_latency_pa_out_manual
                        ? latency_pa_out / 1000.0
                        : outdevinfo.defaultLowOutputLatency;
                    //  double pa_out_latency = vac1_latency_pa_out_manual ?
                    //  latency_pa_out / 1000.0 : outp_dev2.;

                    if (vac_output_iq)
                    {
                        num_chan = 2;
                        sample_rate = sample_rate1;
                        block_size = block_size1;
                    }
                    else if (vac_stereo)
                        num_chan = 2;
                    VACRBReset = true;

                    ivac.SetIVAChostAPIindex(0, host2);
                    ivac.SetIVACinputDEVindex(0, input_dev2);
                    ivac.SetIVACoutputDEVindex(0, output_dev2);
                    ivac.SetIVACnumChannels(0, num_chan);
                    ivac.SetIVACInLatency(0, in_latency, 0);
                    ivac.SetIVACOutLatency(0, out_latency, 0);
                    ivac.SetIVACPAInLatency(0, pa_in_latency, 0);
                    ivac.SetIVACPAOutLatency(0, pa_out_latency, 1);
                    int return_value
                        = Convert.ToInt32(PortAudioForThetis.PaErrorCode.paNoError);

                    Debug.Assert(Audio.console != null);
                    Debug.Assert(Audio.console.SetupForm != null);
                    Audio.console.SetupForm.showActualPALatency(false);

                    try
                    {
                        return_value = ivac.StartAudioIVAC(0);
                        bool retval = return_value
                            == Convert.ToInt32(
                                PortAudioForThetis.PaErrorCode.paNoError);
                        if (!retval)
                        {
                            console.SetupForm.showActualPALatency(false);
                            pa_msg
                            = "\n\nFailed to start VAC. Audio subsystem reports: "
                            + PortAudioForThetis.PA_GetErrorText(return_value);
                            var v = PortAudioForThetis.PA_GetLastHostErrorInfo();
                            if (v.errorText.Length > 0)
                                pa_msg += "\n\n Api Error: " + v.errorText;
                        }
                        if (retval && console.PowerOn)
                        {

                            ivac.SetIVACrun(0, 1);
                            console.SetupForm.showActualPALatency();
                        }
                        else
                        {
                            Status[0].status = pa_msg;
                            Status[0].state = false;

                            throw new Exception("VAC audio engine failed to start");
                        }
                    }
                    catch (Exception)
                    {

                        if (return_value != 0)
                        {
                            pa_msg
                                = "\n\nFailed to start VAC. Audio subsystem reports: "
                                + PortAudioForThetis.PA_GetErrorText(return_value);
                            var v = PortAudioForThetis.PA_GetLastHostErrorInfo();
                            if (v.errorText.Length > 0)
                                pa_msg += "\n\n Api Error: " + v.errorText;

                            if ((PortAudioForThetis.PaErrorCode)(return_value)
                                == PortAudioForThetis.PaErrorCode
                                       .paInvalidSampleRate)
                            {
                                // pa_msg += "\n" + "Suggested sample rate (for
                                // input) is: " +
                                // PortAudioForThetis.PA_GetDeviceInfo(input_dev2).defaultSampleRate
                                //     + "\n" + "Suggested sample rate (for output)
                                //     is: " +
                                //     PortAudioForThetis.PA_GetDeviceInfo(output_dev2).defaultSampleRate;
                            }
                        }
                        Status[0].status = pa_msg;
                        Status[0].state = false;

                        MessageBox.Show(
                            "The program is having trouble starting the VAC audio streams.\n"
                                + "Please examine the VAC related settings on the Setup Form -> Audio Tab and try again."
                                + pa_msg,
                            "VAC Audio Stream Startup Error.", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            else
            {
                ivac.SetIVACrun(0, 0);
                ivac.StopAudioIVAC(0);
                Status[0].status = "";
                Status[0].state = false;
                if (console != null && console.SetupForm != null)
                {
                    console.SetupForm.showActualPALatency(false);
                }
            }
        }

        public static void EnableVAC2(bool enable)
        {
            bool retval = false;

            if (enable) unsafe
                {
                    Status[1].state = true;
                    Status[1].status = "";
                    int num_chan = 1;
                    int sample_rate = sample_rate3;
                    int block_size = block_size_vac2;

                    double in_latency = vac2_latency_manual
                        ? latency3 / 1000.0
                        : PortAudioForThetis.PA_GetDeviceInfo(input_dev3)
                              .defaultLowInputLatency;
                    double out_latency = vac2_latency_out_manual
                        ? vac2_latency_out / 1000.0
                        : PortAudioForThetis.PA_GetDeviceInfo(output_dev3)
                              .defaultLowOutputLatency;
                    double pa_in_latency = vac2_latency_pa_in_manual
                        ? vac2_latency_pa_in / 1000.0
                        : PortAudioForThetis.PA_GetDeviceInfo(input_dev3)
                              .defaultLowInputLatency;
                    double pa_out_latency = vac2_latency_pa_out_manual
                        ? vac2_latency_pa_out / 1000.0
                        : PortAudioForThetis.PA_GetDeviceInfo(output_dev3)
                              .defaultLowOutputLatency;

                    if (vac2_output_iq)
                    {
                        num_chan = 2;
                        sample_rate = sample_rate_rx2;
                        block_size = block_size_rx2;
                    }
                    else if (vac2_stereo)
                        num_chan = 2;

                    VAC2RBReset = true;

                    ivac.SetIVAChostAPIindex(1, host3);
                    ivac.SetIVACinputDEVindex(1, input_dev3);
                    ivac.SetIVACoutputDEVindex(1, output_dev3);
                    ivac.SetIVACnumChannels(1, num_chan);
                    ivac.SetIVACInLatency(1, in_latency, 0);
                    ivac.SetIVACOutLatency(1, out_latency, 0);
                    ivac.SetIVACPAInLatency(1, pa_in_latency, 0);
                    ivac.SetIVACPAOutLatency(1, pa_out_latency, 1);

                    try
                    {
                        retval = ivac.StartAudioIVAC(1) == 1;
                        if (retval && console.PowerOn) ivac.SetIVACrun(1, 1);

                    }
                    catch (Exception)
                    {
                        Status[0].state = false;
                        Status[0].status = "Failed to start VAC2";
                        MessageBox.Show(
                            "The program is having trouble starting the VAC2 audio streams.\n"
                                + "Please examine the VAC related settings on the Setup Form -> Audio Tab and try again.",
                            "VAC2 Audio Stream Startup Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            else
            {
                ivac.SetIVACrun(1, 0);
                ivac.StopAudioIVAC(1);
                Status[0].state = false;
                Status[0].status = "";
            }
        }

        public static bool Start()
        {
            bool retval = false;
            int rc = 0;
            Console c = Console.getConsole();

            try
            {

                phase_buf_l = new float[2048];
                phase_buf_r = new float[2048];

                c.pause_DisplayThread = true;
                rc = NetworkIO.initRadio();

                if (rc != 0)
                {
                    if (rc == -101) // firmware version error;
                    {
                        string fw_err = NetworkIO.getFWVersionErrorMsg();
                        if (fw_err == null)
                        {
                            fw_err = "Bad Firmware levels";
                        }
                        MessageBox.Show(fw_err, "Firmware Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        MessageBox.Show(
                            "Error starting SDR hardware, is it connected and powered?",
                            "Network Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }
                }

                double peakval = 0;
                bool setpeak = false;
                var saved = Common.GetSavedPSPeakValue();
                if (saved.Length > 0)
                {
                    if (Double.TryParse(saved, out peakval))
                    {
                        puresignal.SetPSHWPeak(
                            cmaster.chid(cmaster.inid(1, 0), 0), peakval);
                        setpeak = true;
                    }
                }

                // add setup calls that are needed to change between P1 & P2 before
                // startup
                if (NetworkIO.CurrentRadioProtocol == RadioProtocol.USB)
                {
                    c.SetupForm.lblProt.Text = "Current Protocol: P1";
                    console.SampleRateTX = 48000; // set tx audio sampling rate
                    WDSP.SetTXACFIRRun(cmaster.chid(cmaster.inid(1, 0), 0), false);

                    if (!setpeak) peakval = 0.4072;

                    if (!setpeak && Common.RadioModel == HPSDRModel.HERMES)
                    {
                        peakval = 0.243290600682013;
                        puresignal.SetPSHWPeak(
                            cmaster.chid(cmaster.inid(1, 0), 0), peakval);
                        setpeak = true;
                    }

                    if (!setpeak)
                    {
                        peakval = 0.4072;
                        puresignal.SetPSHWPeak(
                            cmaster.chid(cmaster.inid(1, 0), 0), peakval);
                    }

                    console.psform.PSdefpeak = Convert.ToString(peakval);
                }
                else
                {
                    c.SetupForm.lblProt.Text = "Current Protocol: P2";
                    console.SampleRateTX = 192000;
                    WDSP.SetTXACFIRRun(cmaster.chid(cmaster.inid(1, 0), 0), true);
                    if (!setpeak)
                    {
                        puresignal.SetPSHWPeak(
                            cmaster.chid(cmaster.inid(1, 0), 0), 0.2899);
                        setpeak = true;
                    }
                    console.psform.PSdefpeak = "0.2899";
                }

                Debug.Assert(setpeak);
                c.SetupForm.InitAudioTab();
                c.SetupForm.ForceReset = true;
                cmaster.PSLoopback = cmaster.PSLoopback;

                int result = NetworkIO.StartAudioNative();
                if (result == 0) retval = true;
            }
            catch (Exception e)
            {
                c.SetupForm.lblProt.Text = "None";
                Common.LogException(e);
                MessageBox.Show(e.Message);
            }
            finally
            {
                c.pause_DisplayThread = false;
            }

            return retval;
        }

        //        private static void PortAudioErrorMessageBox(PaErrorCode error)
        //       {
        //            if (error < PaErrorCode.NoError) throw
        //            PortAudioException.GetException(error);
        //             switch (error)
        //             {
        //                 case PaErrorCode.InvalidDevice:
        //                     string s = "Whoops!  Looks like something has gone
        //                     wrong in the\n" +
        //                         "Audio section.  Go look in the Setup Form ->
        //                         Audio Tab to\n" + "verify the settings there.";
        //                     if (vac_enabled) s += "  Since VAC is enabled, make
        //                     sure\n" +
        //                          "you look at those settings as well.";
        //                     MessageBox.Show(s, "Audio Subsystem Error: Invalid
        //                     Device",
        //                         MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                     break;
        //                 default:
        //                     MessageBox.Show(PortAudio.Pa_GetErrorText(error),
        //                     "PortAudio Error: " + error,
        //                         MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                     break;
        //             }
        //        }

        #endregion

        #region Scope Stuff

        private static int scope_samples_per_pixel = 10000;

        public static int ScopeSamplesPerPixel
        {
            get { return scope_samples_per_pixel; }
            set { scope_samples_per_pixel = value; }
        }

        private static int scope_display_width = 704;

        public static int ScopeDisplayWidth
        {
            get { return scope_display_width; }
            set { scope_display_width = value; }
        }

        private static int scope_sample_index;
        private static int scope_pixel_index;
        private static float scope_pixel_min = float.MaxValue;
        private static float scope_pixel_max = float.MinValue;
        private static float[] scope_min;

        public static float[] ScopeMin
        {
            set { scope_min = value; }
        }

        public static float[] scope_max;

        public static float[] ScopeMax
        {
            set { scope_max = value; }
        }

        unsafe public static void DoScope(float* buf, int frameCount)
        {
            if (scope_min == null || scope_min.Length < scope_display_width)
            {
                if (Display.ScopeMin == null
                    || Display.ScopeMin.Length < scope_display_width)
                    Display.ScopeMin = new float[scope_display_width];
                scope_min = Display.ScopeMin;
            }
            if (scope_max == null || scope_max.Length < scope_display_width)
            {
                if (Display.ScopeMax == null
                    || Display.ScopeMax.Length < scope_display_width)
                    Display.ScopeMax = new float[scope_display_width];
                scope_max = Display.ScopeMax;
            }

            for (int i = 0; i < frameCount; i++)
            {
                if (Display.CurrentDisplayMode == DisplayMode.SCOPE
                    || // MW0LGE added all other scope modes
                    Display.CurrentDisplayMode == DisplayMode.PANASCOPE
                    || Display.CurrentDisplayMode == DisplayMode.SPECTRASCOPE)
                {
                    if (buf[i] < scope_pixel_min) scope_pixel_min = buf[i];
                    if (buf[i] > scope_pixel_max) scope_pixel_max = buf[i];
                }
                else
                {
                    scope_pixel_min = buf[i];
                    scope_pixel_max = buf[i];
                }

                scope_sample_index++;
                if (scope_sample_index >= scope_samples_per_pixel)
                {
                    scope_sample_index = 0;
                    scope_min[scope_pixel_index] = scope_pixel_min;
                    scope_max[scope_pixel_index] = scope_pixel_max;

                    scope_pixel_min = float.MaxValue;
                    scope_pixel_max = float.MinValue;

                    scope_pixel_index++;
                    if (scope_pixel_index >= scope_display_width)
                        scope_pixel_index = 0;
                }
            }
        }

        #endregion

        #region Scope2 Stuff

        private static int scope2_sample_index;
        private static int scope2_pixel_index;
        private static float scope2_pixel_min = float.MaxValue;
        private static float scope2_pixel_max = float.MinValue;
        public static float[] scope2_max;
        private static float[] scope2_min;

        public static float[] Scope2Max
        {
            set { scope2_max = value; }
        }
        public static float[] Scope2Min
        {
            set { scope2_min = value; }
        }

        unsafe public static void DoScope2(float* buf, int frameCount)
        {
            if (scope2_min == null || scope2_min.Length < scope_display_width)
            {
                if (Display.Scope2Min == null
                    || Display.Scope2Min.Length < scope_display_width)
                    Display.Scope2Min = new float[scope_display_width];
                scope2_min = Display.Scope2Min;
            }
            if (scope2_max == null || scope2_max.Length < scope_display_width)
            {
                if (Display.Scope2Max == null
                    || Display.Scope2Max.Length < scope_display_width)
                    Display.Scope2Max = new float[scope_display_width];
                scope2_max = Display.Scope2Max;
            }

            for (int i = 0; i < frameCount; i++)
            {
                if (Display.CurrentDisplayMode == DisplayMode.SCOPE
                    || Display.CurrentDisplayMode == DisplayMode.PANASCOPE
                    || Display.CurrentDisplayMode == DisplayMode.SPECTRASCOPE)
                {
                    if (buf[i] < scope2_pixel_min) scope2_pixel_min = buf[i];
                    if (buf[i] > scope2_pixel_max) scope2_pixel_max = buf[i];
                }
                else
                {
                    scope2_pixel_min = buf[i];
                    scope2_pixel_max = buf[i];
                }

                scope2_sample_index++;
                if (scope2_sample_index >= scope_samples_per_pixel)
                {
                    scope2_sample_index = 0;
                    scope2_min[scope_pixel_index] = scope2_pixel_min;
                    scope2_max[scope_pixel_index] = scope2_pixel_max;

                    scope2_pixel_min = float.MaxValue;
                    scope2_pixel_max = float.MinValue;

                    scope2_pixel_index++;
                    if (scope2_pixel_index >= scope_display_width)
                        scope2_pixel_index = 0;
                }
            }
        }

        #endregion
    }
}
