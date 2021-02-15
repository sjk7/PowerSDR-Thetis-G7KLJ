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

namespace Thetis {

public class PrettySMeterHelpers

{

    private Console m_console;
    private MeterRXMode m_meterRxMode;
    private MeterTXMode m_meterTXMode;
    private bool m_Mox = false;
    private frmSMeter m_frmSMeter;

    private ImageStruct m_Background;

    private ImageStruct m_AudioBackground;
    private metering.helpers m_helper;
    public PrettySMeterHelpers(Thetis.Console console) {

        m_console = console;
        m_helper = new metering.helpers(console);
        test();

        ImageStructEx img = new ImageStructEx();
        img.data.scaleKind = MeterScaleKinds.log_scale;
        img.bitmap = helpers.makeBitmap(Resources.PPM);
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

    public void rxMeterModeChanged(MeterRXMode newMode) {
        m_meterRxMode = newMode;
    }

    public void txMeterModeChanged(MeterTXMode newMode) {
        m_meterTXMode = newMode;
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
