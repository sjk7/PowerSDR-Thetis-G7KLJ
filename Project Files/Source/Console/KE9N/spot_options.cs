//=================================================================
// spot_options.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2003-2013  FlexRadio Systems
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
// You may contact us via email at: gpl@flexradio.com.
// Paper mail may be sent to: 
//    FlexRadio Systems
//    4616 W. Howard Lane  Suite 1-150
//    Austin, TX 78728
//    USA
//=================================================================

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Thetis
{
    /// <summary>
    /// Summary description for WaveOptions.
    /// </summary>
    public class SpotOptions : Form
    {
        #region Variable Declaration
        private ToolTip toolTip1;
        public NumericUpDownTS udMethod;
        private Label label5;
        public Label label1;
        public NumericUpDownTS udSSN;
        public NumericUpDownTS udMTA;
        public Label label3;
        public NumericUpDownTS udRCR;
        public Label label4;
        public NumericUpDownTS udSNR;
        public Label label6;
        public NumericUpDownTS udWATTS;
        public Label label7;
        private IContainer components;
        public NumericUpDownTS udDAY;
        public Label label8;
        private Button btnTrack;
        private TextBox textBox1;
        public static SpotControl SpotForm;                       // ke9ns add DX spotter function

        #endregion

        #region Constructor and Destructor

        public SpotOptions()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Common.RestoreForm(this, "SpotOptions", false);


        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SpotOptions));
            this.toolTip1 = new ToolTip(this.components);
            this.btnTrack = new Button();
            this.udDAY = new NumericUpDownTS();
            this.udWATTS = new NumericUpDownTS();
            this.udSNR = new NumericUpDownTS();
            this.udRCR = new NumericUpDownTS();
            this.udMTA = new NumericUpDownTS();
            this.udSSN = new NumericUpDownTS();
            this.udMethod = new NumericUpDownTS();
            this.label5 = new Label();
            this.label1 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.label8 = new Label();
            this.textBox1 = new TextBox();
            ((ISupportInitialize)(this.udDAY)).BeginInit();
            ((ISupportInitialize)(this.udWATTS)).BeginInit();
            ((ISupportInitialize)(this.udSNR)).BeginInit();
            ((ISupportInitialize)(this.udRCR)).BeginInit();
            ((ISupportInitialize)(this.udMTA)).BeginInit();
            ((ISupportInitialize)(this.udSSN)).BeginInit();
            ((ISupportInitialize)(this.udMethod)).BeginInit();
            this.SuspendLayout();
            // 
            // btnTrack
            // 
            this.btnTrack.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTrack.Location = new Point(28, 211);
            this.btnTrack.Name = "btnTrack";
            this.btnTrack.Size = new Size(75, 23);
            this.btnTrack.TabIndex = 99;
            this.btnTrack.Text = "UPDATE";
            this.toolTip1.SetToolTip(this.btnTrack, "Update VOACAP map ");
            this.btnTrack.UseVisualStyleBackColor = true;
            this.btnTrack.Click += new EventHandler(this.btnTrack_Click);
            // 
            // udDAY
            // 
            this.udDAY.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udDAY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDAY.Location = new Point(73, 173);
            this.udDAY.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.udDAY.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udDAY.Name = "udDAY";
            this.udDAY.Size = new Size(46, 20);
            this.udDAY.TabIndex = 97;
            this.toolTip1.SetToolTip(this.udDAY, "Settings the correct Day of the month uses URSI coefficients\r\nSetting the Day to " +
        "0 uses CCRI coefficients (which are considered to be better)\r\n");
            this.udDAY.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udDAY.ValueChanged += new EventHandler(this.numericUpDownTS1_ValueChanged);
            // 
            // udWATTS
            // 
            this.udWATTS.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udWATTS.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udWATTS.Location = new Point(73, 147);
            this.udWATTS.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.udWATTS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udWATTS.Name = "udWATTS";
            this.udWATTS.Size = new Size(59, 20);
            this.udWATTS.TabIndex = 95;
            this.toolTip1.SetToolTip(this.udWATTS, "Watts 1 to 1500");
            this.udWATTS.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.udWATTS.ValueChanged += new EventHandler(this.udWATTS_ValueChanged);
            // 
            // udSNR
            // 
            this.udSNR.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udSNR.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSNR.Location = new Point(73, 121);
            this.udSNR.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.udSNR.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udSNR.Name = "udSNR";
            this.udSNR.Size = new Size(46, 20);
            this.udSNR.TabIndex = 93;
            this.toolTip1.SetToolTip(this.udSNR, "SNR dbm\r\nlower dbm values for CW (45) , higher values for AM (75)");
            this.udSNR.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udSNR.ValueChanged += new EventHandler(this.udSNR_ValueChanged);
            // 
            // udRCR
            // 
            this.udRCR.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udRCR.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udRCR.Location = new Point(73, 95);
            this.udRCR.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.udRCR.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udRCR.Name = "udRCR";
            this.udRCR.Size = new Size(46, 20);
            this.udRCR.TabIndex = 91;
            this.toolTip1.SetToolTip(this.udRCR, "Required Circuit reliability: default is 90%");
            this.udRCR.Value = new decimal(new int[] {
            89,
            0,
            0,
            0});
            this.udRCR.ValueChanged += new EventHandler(this.udRCR_ValueChanged);
            // 
            // udMTA
            // 
            this.udMTA.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udMTA.DecimalPlaces = 3;
            this.udMTA.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udMTA.Location = new Point(73, 69);
            this.udMTA.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.udMTA.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udMTA.Name = "udMTA";
            this.udMTA.Size = new Size(59, 20);
            this.udMTA.TabIndex = 89;
            this.toolTip1.SetToolTip(this.udMTA, "Min Takeoff Angle: default is normally 0.100 up to 3.000\r\n");
            this.udMTA.Value = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            this.udMTA.ValueChanged += new EventHandler(this.udMTA_ValueChanged);
            // 
            // udSSN
            // 
            this.udSSN.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udSSN.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSSN.Location = new Point(73, 39);
            this.udSSN.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.udSSN.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udSSN.Name = "udSSN";
            this.udSSN.Size = new Size(46, 20);
            this.udSSN.TabIndex = 88;
            this.toolTip1.SetToolTip(this.udSSN, "Enter the Smoothed Sunspot number based on SFI\r\nThis number is greatly reduced wh" +
        "en the K index rises");
            this.udSSN.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.udSSN.ValueChanged += new EventHandler(this.udSSN_ValueChanged);
            // 
            // udMethod
            // 
            this.udMethod.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.udMethod.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMethod.Location = new Point(73, 13);
            this.udMethod.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.udMethod.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udMethod.Name = "udMethod";
            this.udMethod.Size = new Size(46, 20);
            this.udMethod.TabIndex = 80;
            this.toolTip1.SetToolTip(this.udMethod, "Method: Voacap has different computation methods. \r\n30 is normally the default");
            this.udMethod.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.udMethod.ValueChanged += new EventHandler(this.udMethod_ValueChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new Point(12, 13);
            this.label5.Name = "label5";
            this.label5.Size = new Size(43, 13);
            this.label5.TabIndex = 85;
            this.label5.Text = "Method";
            // 
            // label1
            // 
            this.label1.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new Size(29, 13);
            this.label1.TabIndex = 86;
            this.label1.Text = "SSN";
            // 
            // label3
            // 
            this.label3.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new Size(30, 13);
            this.label3.TabIndex = 90;
            this.label3.Text = "MTA";
            // 
            // label4
            // 
            this.label4.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new Point(11, 102);
            this.label4.Name = "label4";
            this.label4.Size = new Size(38, 13);
            this.label4.TabIndex = 92;
            this.label4.Text = "RCR%";
            // 
            // label6
            // 
            this.label6.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new Point(11, 128);
            this.label6.Name = "label6";
            this.label6.Size = new Size(30, 13);
            this.label6.TabIndex = 94;
            this.label6.Text = "SNR";
            // 
            // label7
            // 
            this.label7.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new Point(11, 154);
            this.label7.Name = "label7";
            this.label7.Size = new Size(35, 13);
            this.label7.TabIndex = 96;
            this.label7.Text = "Watts";
            // 
            // label8
            // 
            this.label8.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new Point(12, 180);
            this.label8.Name = "label8";
            this.label8.Size = new Size(26, 13);
            this.label8.TabIndex = 98;
            this.label8.Text = "Day";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Point(161, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(194, 222);
            this.textBox1.TabIndex = 100;
            this.textBox1.Text = "When this Window is Open, all VOACAP settings come from this window except Freqeu" +
    "ncy, Month, Hour\r\n\r\nClose this Window to use default VOCAP setup from dx spotter" +
    " window.";
            // 
            // SpotOptions
            // 
            this.AutoScaleBaseSize = new Size(5, 13);
            this.ClientSize = new Size(367, 246);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnTrack);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.udDAY);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.udWATTS);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.udSNR);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.udRCR);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.udMTA);
            this.Controls.Add(this.udSSN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.udMethod);
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SpotOptions";
            this.Text = "VOACAP Override";
            this.Closing += new CancelEventHandler(this.SpotOptions_Closing);
            ((ISupportInitialize)(this.udDAY)).EndInit();
            ((ISupportInitialize)(this.udWATTS)).EndInit();
            ((ISupportInitialize)(this.udSNR)).EndInit();
            ((ISupportInitialize)(this.udRCR)).EndInit();
            ((ISupportInitialize)(this.udMTA)).EndInit();
            ((ISupportInitialize)(this.udSSN)).EndInit();
            ((ISupportInitialize)(this.udMethod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Properties






        #endregion

        #region Event Handler

        private void SpotOptions_Closing(object sender, CancelEventArgs e)
        {
            SpotForm.VOACAP_FORCE = false;
            this.Hide();
            e.Cancel = true;
            Common.SaveForm(this, "SpotOptions");
        }



        #endregion

        private void udMethod_ValueChanged(object sender, EventArgs e)
        {
            //  SpotForm.VOACAP_FORCE = true;
            //   SpotForm.VOACAP_CHECK();
        }

        private void udSSN_ValueChanged(object sender, EventArgs e)
        {
            // SpotForm.VOACAP_FORCE = true;
            //  SpotForm.VOACAP_CHECK();
        }

        private void udMTA_ValueChanged(object sender, EventArgs e)
        {
            //  SpotForm.VOACAP_FORCE = true;
            //   SpotForm.VOACAP_CHECK();
        }

        private void udRCR_ValueChanged(object sender, EventArgs e)
        {
            //  SpotForm.VOACAP_FORCE = true;
            //  SpotForm.VOACAP_CHECK();
        }

        private void udSNR_ValueChanged(object sender, EventArgs e)
        {
            //  SpotForm.VOACAP_FORCE = true;
            //  SpotForm.VOACAP_CHECK();
        }

        private void udWATTS_ValueChanged(object sender, EventArgs e)
        {
            //  SpotForm.VOACAP_FORCE = true;
            //  SpotForm.VOACAP_CHECK();

        }

        private void numericUpDownTS1_ValueChanged(object sender, EventArgs e)
        {
            //   SpotForm.VOACAP_FORCE = true;
            //  SpotForm.VOACAP_CHECK();
        }

        private void btnTrack_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("OPTIONS UPDATE HERE0");

            SpotForm.VOACAP_FORCE = true;

            Debug.WriteLine("OPTIONS UPDATE HERE");

            SpotForm.VOACAP_CHECK();
        }
    } // SpotOptions

} // PowerSDR
