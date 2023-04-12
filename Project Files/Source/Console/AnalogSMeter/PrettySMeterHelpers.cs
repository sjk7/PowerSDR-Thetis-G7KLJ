using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thetis;
using Thetis.Properties;
using System.Drawing;
using Thetis.metering;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using LBSoft.IndustrialCtrls.Meters;

namespace Thetis {

public class PrettySMeterHelpers

{

    private readonly Console m_console;
    private MeterRXMode m_meterRxMode;
    private MeterTXMode m_meterTXMode;
    private bool m_Mox = false;
    private frmSMeter m_frmSMeter;

    private ImageStruct m_Background;

    private ImageStruct m_AudioBackground;
    private helpers m_helper;
    public PrettySMeterHelpers(Console console) {

        m_console = console;
        m_helper = new helpers(console);
        test();

        ImageStructEx img = new ImageStructEx();
        img.data.scaleKind = MeterScaleKinds.log_scale;
        img.bitmap = helpers.makeBitmap(Resources.PPM);
    }

    public static void showSWR(LBAnalogMeter.BackGroundChoices bkg, double val,
        LBAnalogMeter PrettySMeter, Console c) {

        // val is a float representing SWR, multiplied by 100.
        // Debug.Print(val.ToString());
        PrettySMeter.MinValue = 0;
        PrettySMeter.MaxValue = 600;
        var below = 160 - val;
        if (below > 1.0) {
            val -= (below * 4);
            if (val < 0) {
                val = 0;
            }
        }

        // Debug.Print(val.ToString());

        PrettySMeter.Value = val;
    }

    public static void showPower(LBAnalogMeter.BackGroundChoices bkg,
        double val, LBAnalogMeter PrettySMeter, Console c) {
        PrettySMeter.MinValue = 0;
        double maxScale = 150; // <-- whatever the meter shows as max power.
                               // NOTE: for Herpes-Lite, when calibrating power,
                               // make the power read actual power * 100
        switch (c.CurrentHPSDRModel) {

            case HPSDRModel.ANAN100:
            case HPSDRModel.ANAN100D:

                if (bkg == LBAnalogMeter.BackGroundChoices.Tango) {
                    maxScale = 250;
                } else {
                    maxScale = 150;
                }

                break;
            case HPSDRModel.HERMES:
            case HPSDRModel.ANAN10:
            case HPSDRModel.ANAN10E:
                switch (bkg) {
                    case LBAnalogMeter.BackGroundChoices.Tango:
                        maxScale = 250;
                        break;

                    default: maxScale = 150; break;
                }

                break;
            default: maxScale = 150; break;
        }

        var fudge = 0.95;
        PrettySMeter.MaxValue = maxScale * 100;
        switch (bkg) {
            case LBAnalogMeter.BackGroundChoices.NewVFOAnalogSignalGauge:
                fudge = 1.0;
                break;
            case LBAnalogMeter.BackGroundChoices.Blue: fudge = 1.0; break;

            case LBAnalogMeter.BackGroundChoices.Tango: fudge = 0.8; break;
            case LBAnalogMeter.BackGroundChoices.Kenwood: fudge = 3.5; break;

            default: fudge = 1.0; break;
        }
        val *= 10;
        PrettySMeter.MaxValue = PrettySMeter.MaxValue * fudge;
        PrettySMeter.Value = val;
    }

    private void test() {
        ImageStructEx bkg = new ImageStructEx();

        bkg.data.fileName = "test";
        bkg.resourceId = BuiltInMeters.Blue;
        bkg.data.scaleKind = MeterScaleKinds.log_scale;
        m_helper.ImageStructExSerialize(bkg);

        ImageStructEx imgbak = m_helper.ImageStructExDeSerialize(
            bkg.data.meterType, bkg.data.resourceid);

        Debug.Assert(imgbak.data.fileName == "test");
        Debug.Assert(imgbak.bitmap != null);
        Debug.Assert(imgbak.data.resourceid == BuiltInMeters.Blue);
        Debug.Assert(imgbak.data.scaleKind == MeterScaleKinds.log_scale);
    }
    public void moxChanged(bool mox) { m_Mox = mox; }

    // we seem to get this as we go into and out of mox
    public void rxMeterModeChanged(MeterRXMode newMode, bool goingIntoMOX) {
        if (goingIntoMOX) return;
        int which = Settings.Default.SMeterBackgroundImg;
        var choice
            = (LBSoft.IndustrialCtrls.Meters.LBAnalogMeter.BackGroundChoices)
                which;

        m_meterRxMode = newMode;
        if (this.m_console != null) {
            var f = m_console.BigSMeter;
            if (f != null) {
                f.BigSMeter.ToggleBackGroundImage(choice);
            }
            m_console.PrettySMeter.ToggleBackGroundImage(choice);
        }
    }

    // we seem to get this as we go into and out of mox
    public void txMeterModeChanged(MeterTXMode newMode, bool goingIntoMOX) {
        if (!goingIntoMOX) return;

        int which = Settings.Default.SMeterBackgroundImg;
        var choice
            = (LBSoft.IndustrialCtrls.Meters.LBAnalogMeter.BackGroundChoices)
                which;

        m_meterTXMode = newMode;
        if (this.m_console != null) {
            switch (m_meterTXMode) {
                case MeterTXMode.MIC:
                    case MeterTXMode.ALC:
                    case MeterTXMode.LEVELER:
                    case MeterTXMode.CFC_PK:
                    case MeterTXMode.COMP:

                        choice = LBSoft.IndustrialCtrls.Meters.LBAnalogMeter
                                 .BackGroundChoices.VU;
                    break;

                case MeterTXMode.FORWARD_POWER:
                case MeterTXMode.REVERSE_POWER:
                    // leave choice as-is
                    break;

                default: Debug.Print(m_meterTXMode.ToString()); break;
            }

            var f = m_console.BigSMeter;
            if (f != null) {
                f.BigSMeter.ToggleBackGroundImage(choice);
            }
            m_console.PrettySMeter.ToggleBackGroundImage(choice);
        }
    }
    public frmSMeter frmSMeter {
        get { return m_frmSMeter; }
        set { m_frmSMeter = value; }
    }

    public ImageStruct AudioMeterBackground {
        get { return m_AudioBackground; }
        set { m_AudioBackground = value; }
    }

    public ImageStruct SMeterBackground {
        get { return m_Background; }
        set { m_Background = value; }
    }
}

}
