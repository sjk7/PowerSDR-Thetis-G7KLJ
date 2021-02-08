
namespace Thetis
{
    partial class frmSMeter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSMeter));
            this.BigSMeter = new LBSoft.IndustrialCtrls.Meters.LBAnalogMeter();
            this.SuspendLayout();
            // 
            // BigSMeter
            // 
            this.BigSMeter.BodyColor = System.Drawing.Color.Red;
            this.BigSMeter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BigSMeter.Location = new System.Drawing.Point(0, 0);
            this.BigSMeter.Max = 1000D;
            this.BigSMeter.MaxValue = 1000D;
            this.BigSMeter.MeterStyle = LBSoft.IndustrialCtrls.Meters.LBAnalogMeter.AnalogMeterStyle.Circular;
            this.BigSMeter.MinValue = 0D;
            this.BigSMeter.Name = "BigSMeter";
            this.BigSMeter.NeedleColor = System.Drawing.Color.Yellow;
            this.BigSMeter.Renderer = null;
            this.BigSMeter.ScaleColor = System.Drawing.Color.White;
            this.BigSMeter.ScaleDivisions = 10;
            this.BigSMeter.ScaleSubDivisions = 10;
            this.BigSMeter.Size = new System.Drawing.Size(800, 450);
            this.BigSMeter.TabIndex = 147;
            this.BigSMeter.Value = 0D;
            this.BigSMeter.ViewGlass = true;
            // 
            // frmSMeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BigSMeter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSMeter";
            this.Text = "G7KLJ PowerSDR S Meter Window";
            this.ResumeLayout(false);

        }

        #endregion

        private LBSoft.IndustrialCtrls.Meters.LBAnalogMeter BigSMeter;
    }
}