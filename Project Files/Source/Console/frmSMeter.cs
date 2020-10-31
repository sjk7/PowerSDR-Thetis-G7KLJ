using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thetis
{
    public partial class frmSMeter : Form
    {
        private Thetis.PowerSDR.SMeter Meter;
        protected override void OnResize(EventArgs e)

        {

            base.OnResize(e);
            if (Meter != null)
            {
                Meter.Width = this.Width;
                Meter.Height = this.Height;
            }

        }

        private void frmSMeter_Paint(object sender, PaintEventArgs e)
        {
            if (Meter != null)
                Meter.PaintGauge(e);
        }

        public frmSMeter()
        {
            InitializeComponent();
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmSMeter_Paint);
            this.DoubleBuffered = true;
            this.Visible = true;
            this.BringToFront();
            Meter = new Thetis.PowerSDR.SMeter(Common.Console,1, this);
            Meter.Width = Width;
            Meter.Height = Height;
        }


        public float Value
        {
            set
            {
                Meter.Value = value;
            }
        }
    }


}
