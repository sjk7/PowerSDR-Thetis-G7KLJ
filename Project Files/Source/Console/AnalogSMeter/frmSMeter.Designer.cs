
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSMeter));
            this.mnuBigSMeter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chooseBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BigSMeter = new LBSoft.IndustrialCtrls.Meters.LBAnalogMeter();
            this.mnuBigSMeter.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuBigSMeter
            // 
            this.mnuBigSMeter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseBackgroundToolStripMenuItem});
            this.mnuBigSMeter.Name = "mnuBigSMeter";
            this.mnuBigSMeter.Size = new System.Drawing.Size(182, 26);
            // 
            // chooseBackgroundToolStripMenuItem
            // 
            this.chooseBackgroundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalToolStripMenuItem,
            this.blueToolStripMenuItem});
            this.chooseBackgroundToolStripMenuItem.Name = "chooseBackgroundToolStripMenuItem";
            this.chooseBackgroundToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.chooseBackgroundToolStripMenuItem.Text = "Choose Background";
            // 
            // originalToolStripMenuItem
            // 
            this.originalToolStripMenuItem.Name = "originalToolStripMenuItem";
            this.originalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.originalToolStripMenuItem.Text = "Original";
            this.originalToolStripMenuItem.Click += new System.EventHandler(this.originalToolStripMenuItem_Click);
            // 
            // blueToolStripMenuItem
            // 
            this.blueToolStripMenuItem.Name = "blueToolStripMenuItem";
            this.blueToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.blueToolStripMenuItem.Text = "Blue";
            this.blueToolStripMenuItem.Click += new System.EventHandler(this.blueToolStripMenuItem_Click);
            // 
            // BigSMeter
            // 
            this.BigSMeter.BackgroundImage = global::Thetis.Properties.Resources.NewVFOAnalogSignalGauge;
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
            this.BigSMeter.ViewGlass = false;
            this.BigSMeter.BackGndImgChanged += new System.EventHandler(this.BigSMeter_BackGndImgChanged);
            this.BigSMeter.Load += new System.EventHandler(this.BigSMeter_Load);
            this.BigSMeter.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BigSMeter_MouseClick);
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
            this.Load += new System.EventHandler(this.frmSMeter_Load);
            this.mnuBigSMeter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private LBSoft.IndustrialCtrls.Meters.LBAnalogMeter BigSMeter;
        private System.Windows.Forms.ContextMenuStrip mnuBigSMeter;
        private System.Windows.Forms.ToolStripMenuItem chooseBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem originalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blueToolStripMenuItem;
    }
}