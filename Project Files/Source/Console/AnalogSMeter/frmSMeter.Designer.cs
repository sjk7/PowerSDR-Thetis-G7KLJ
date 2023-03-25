
namespace Thetis {
partial class frmSMeter {
  /// <summary>
  /// Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise,
  /// false.</param>
  protected override void Dispose(bool disposing) {
    if (disposing && (components != null)) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

#region Windows Form Designer generated code

  /// <summary>
  /// Required method for Designer support - do not modify
  /// the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSMeter));
            this.mnuBigSMeter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chooseBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whyCantIChooseTheBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.youKnowWhenYouveBeenTangodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotShowBigSMeterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BigSMeter = new LBSoft.IndustrialCtrls.Meters.LBAnalogMeter();
            this.Grip = new System.Windows.Forms.PictureBox();
            this.kenwoodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBigSMeter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grip)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuBigSMeter
            // 
            this.mnuBigSMeter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseBackgroundToolStripMenuItem,
            this.windowStateToolStripMenuItem});
            this.mnuBigSMeter.Name = "mnuBigSMeter";
            this.mnuBigSMeter.Size = new System.Drawing.Size(182, 70);
            // 
            // chooseBackgroundToolStripMenuItem
            // 
            this.chooseBackgroundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whyCantIChooseTheBackgroundToolStripMenuItem,
            this.originalToolStripMenuItem,
            this.blueToolStripMenuItem,
            this.youKnowWhenYouveBeenTangodToolStripMenuItem,
            this.kenwoodToolStripMenuItem});
            this.chooseBackgroundToolStripMenuItem.Name = "chooseBackgroundToolStripMenuItem";
            this.chooseBackgroundToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.chooseBackgroundToolStripMenuItem.Text = "Choose Background";
            // 
            // whyCantIChooseTheBackgroundToolStripMenuItem
            // 
            this.whyCantIChooseTheBackgroundToolStripMenuItem.Name = "whyCantIChooseTheBackgroundToolStripMenuItem";
            this.whyCantIChooseTheBackgroundToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.whyCantIChooseTheBackgroundToolStripMenuItem.Text = "Why Can\'t I choose the background?";
            this.whyCantIChooseTheBackgroundToolStripMenuItem.Click += new System.EventHandler(this.whyCantIChooseTheBackgroundToolStripMenuItem_Click);
            // 
            // originalToolStripMenuItem
            // 
            this.originalToolStripMenuItem.Name = "originalToolStripMenuItem";
            this.originalToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.originalToolStripMenuItem.Text = "Original";
            this.originalToolStripMenuItem.Click += new System.EventHandler(this.originalToolStripMenuItem_Click);
            // 
            // blueToolStripMenuItem
            // 
            this.blueToolStripMenuItem.Name = "blueToolStripMenuItem";
            this.blueToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.blueToolStripMenuItem.Text = "Blue";
            this.blueToolStripMenuItem.Click += new System.EventHandler(this.blueToolStripMenuItem_Click);
            // 
            // youKnowWhenYouveBeenTangodToolStripMenuItem
            // 
            this.youKnowWhenYouveBeenTangodToolStripMenuItem.Name = "youKnowWhenYouveBeenTangodToolStripMenuItem";
            this.youKnowWhenYouveBeenTangodToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.youKnowWhenYouveBeenTangodToolStripMenuItem.Text = "You know when you\'ve been Tango\'d!";
            this.youKnowWhenYouveBeenTangodToolStripMenuItem.Click += new System.EventHandler(this.youKnowWhenYouveBeenTangodToolStripMenuItem_Click);
            // 
            // windowStateToolStripMenuItem
            // 
            this.windowStateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.normalToolStripMenuItem,
            this.minimizedToolStripMenuItem,
            this.maximizedToolStripMenuItem,
            this.alwaysOnTopToolStripMenuItem,
            this.doNotShowBigSMeterToolStripMenuItem});
            this.windowStateToolStripMenuItem.Name = "windowStateToolStripMenuItem";
            this.windowStateToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.windowStateToolStripMenuItem.Text = "Window State";
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.normalToolStripMenuItem.Text = "Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // minimizedToolStripMenuItem
            // 
            this.minimizedToolStripMenuItem.Name = "minimizedToolStripMenuItem";
            this.minimizedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.minimizedToolStripMenuItem.Text = "Minimized";
            this.minimizedToolStripMenuItem.Click += new System.EventHandler(this.minimizedToolStripMenuItem_Click);
            // 
            // maximizedToolStripMenuItem
            // 
            this.maximizedToolStripMenuItem.Name = "maximizedToolStripMenuItem";
            this.maximizedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.maximizedToolStripMenuItem.Text = "Maximized";
            this.maximizedToolStripMenuItem.Click += new System.EventHandler(this.maximizedToolStripMenuItem_Click);
            // 
            // alwaysOnTopToolStripMenuItem
            // 
            this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
            this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.alwaysOnTopToolStripMenuItem.Text = "Always On Top";
            this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_Click);
            // 
            // doNotShowBigSMeterToolStripMenuItem
            // 
            this.doNotShowBigSMeterToolStripMenuItem.Name = "doNotShowBigSMeterToolStripMenuItem";
            this.doNotShowBigSMeterToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.doNotShowBigSMeterToolStripMenuItem.Text = "Do Not Show Big S-Meter";
            this.doNotShowBigSMeterToolStripMenuItem.Click += new System.EventHandler(this.doNotShowBigSMeterToolStripMenuItem_Click);
            // 
            // BigSMeter
            // 
            this.BigSMeter.BackgroundImage = global::Thetis.Properties.Resources.NewVFOAnalogSignalGauge;
            this.BigSMeter.BodyColor = System.Drawing.Color.Red;
            this.BigSMeter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BigSMeter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BigSMeter.Location = new System.Drawing.Point(0, 0);
            this.BigSMeter.Max = 1000D;
            this.BigSMeter.MaxValue = 1000D;
            this.BigSMeter.MeterStyle = LBSoft.IndustrialCtrls.Meters.LBAnalogMeter.AnalogMeterStyle.Circular;
            this.BigSMeter.MinValue = 0D;
            this.BigSMeter.Name = "BigSMeter";
            this.BigSMeter.NeedleColor = System.Drawing.Color.DarkGoldenrod;
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
            this.BigSMeter.DoubleClick += new System.EventHandler(this.BigSMeter_DoubleClick);
            this.BigSMeter.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BigSMeter_MouseClick);
            this.BigSMeter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigSMeter_MouseDown);
            this.BigSMeter.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BigSMeter_MouseMove);
            // 
            // Grip
            // 
            this.Grip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Grip.BackColor = System.Drawing.Color.RoyalBlue;
            this.Grip.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.Grip.Location = new System.Drawing.Point(795, 445);
            this.Grip.Name = "Grip";
            this.Grip.Size = new System.Drawing.Size(4, 4);
            this.Grip.TabIndex = 148;
            this.Grip.TabStop = false;
            this.Grip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Grip_MouseDown);
            this.Grip.MouseEnter += new System.EventHandler(this.Grip_MouseEnter);
            this.Grip.MouseLeave += new System.EventHandler(this.Grip_MouseLeave);
            this.Grip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Grip_MouseMove);
            this.Grip.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Grip_MouseUp);
            // 
            // kenwoodToolStripMenuItem
            // 
            this.kenwoodToolStripMenuItem.Name = "kenwoodToolStripMenuItem";
            this.kenwoodToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.kenwoodToolStripMenuItem.Text = "Kenwood";
            this.kenwoodToolStripMenuItem.Click += new System.EventHandler(this.kenwoodToolStripMenuItem_Click);
            // 
            // frmSMeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Grip);
            this.Controls.Add(this.BigSMeter);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSMeter";
            this.Text = "G7KLJ PowerSDR S Meter Window";
            this.Load += new System.EventHandler(this.frmSMeter_Load);
            this.mnuBigSMeter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Grip)).EndInit();
            this.ResumeLayout(false);

  }

#endregion

  public LBSoft.IndustrialCtrls.Meters.LBAnalogMeter BigSMeter;
  private System.Windows.Forms.ContextMenuStrip mnuBigSMeter;
  private System.Windows.Forms.ToolStripMenuItem chooseBackgroundToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem originalToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem blueToolStripMenuItem;
  private System.Windows.Forms.PictureBox Grip;
  private System.Windows.Forms.ToolStripMenuItem windowStateToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem minimizedToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem maximizedToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem doNotShowBigSMeterToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem whyCantIChooseTheBackgroundToolStripMenuItem;
  private System.Windows.Forms.ToolStripMenuItem youKnowWhenYouveBeenTangodToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kenwoodToolStripMenuItem;
    }
}
