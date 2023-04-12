//=================================================================
// splash.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2009  FlexRadio Systems
// Copyright (C) 2010-2019  Doug Wigley
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
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Thetis {
public class Splash : Form
    {
    // MW0LGE
    [DllImport("user32.dll")]
    static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

#region Variable Declarations

    // Threading
    static Splash ms_frmSplash = null;
    static Thread ms_oThread = null;

    // Fade in and out.
    private double m_dblOpacityIncrement = .07;
    private double m_dblOpacityDecrement = .035;
    private const int TIMER_INTERVAL = 50;

    // Status and progress bar
    private string m_sStatus;
    private double m_dblCompletionFraction = 0;
    private Rectangle m_rProgress;

    // Progress smoothing
    private double m_dblLastCompletionFraction = 0.0;
    private double m_dblPBIncrementPerTimerInterval = .015;

    // Self-calibration support
    private bool m_bFirstLaunch = false;
    private DateTime m_dtStart;
    private bool m_bDTSet = false;
    private int m_iIndex = 1;
    private int m_iActualTicks = 0;
    private ArrayList m_alPreviousCompletionFraction;
    private ArrayList m_alActualTimes = new ArrayList();
    private const string REG_KEY_INITIALIZATION = "Initialization";
    private const string REGVALUE_PB_MILISECOND_INCREMENT = "Increment";
    private const string REGVALUE_PB_PERCENTS = "Percents";
    private LabelTS lblTimeRemaining;
    private System.Windows.Forms.Timer timer1;
    private LabelTS lblStatus;
    private Panel pnlStatus;
    private LabelTS labelTS1;
    private LabelTS labelTS2;
    private LabelTS labelTS3;
    private LabelTS labelTS4;
        private LabelTS labelTS5;
        private System.ComponentModel.IContainer components;

#endregion

#region Constructor and Destructor

    public Splash() {
        InitializeComponent();
        this.Opacity = .00;
        timer1.Interval = TIMER_INTERVAL;
        timer1.Start();
        this.ClientSize = this.BackgroundImage.Size;
        this.ShowInTaskbar = false;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            if (components != null) {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }

#endregion

#region Windows Form Designer generated code

    private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.pnlStatus = new Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelTS1 = new LabelTS();
            this.lblTimeRemaining = new LabelTS();
            this.lblStatus = new LabelTS();
            this.labelTS2 = new LabelTS();
            this.labelTS3 = new LabelTS();
            this.labelTS4 = new LabelTS();
            this.labelTS5 = new LabelTS();
            this.SuspendLayout();
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.White;
            this.pnlStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new Point(0, 399);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new Size(677, 17);
            this.pnlStatus.TabIndex = 2;
            this.pnlStatus.Paint += new PaintEventHandler(this.pnlStatus_Paint);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            // 
            // labelTS1
            // 
            this.labelTS1.BackColor = System.Drawing.Color.Transparent;
            this.labelTS1.Font = new Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTS1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTS1.Image = null;
            this.labelTS1.Location = new Point(338, 20);
            this.labelTS1.Name = "labelTS1";
            this.labelTS1.Size = new Size(89, 16);
            this.labelTS1.TabIndex = 3;
            this.labelTS1.Text = "De G7KLJ";
            this.labelTS1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeRemaining.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeRemaining.Font = new Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeRemaining.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTimeRemaining.Image = null;
            this.lblTimeRemaining.Location = new Point(586, 380);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new Size(79, 16);
            this.lblTimeRemaining.TabIndex = 1;
            this.lblTimeRemaining.Text = "Starting ...";
            this.lblTimeRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblStatus.Image = null;
            this.lblStatus.Location = new Point(262, 380);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(303, 16);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTS2
            // 
            this.labelTS2.AutoSize = true;
            this.labelTS2.BackColor = System.Drawing.Color.Transparent;
            this.labelTS2.Font = new Font("Arial", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTS2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTS2.Image = null;
            this.labelTS2.Location = new Point(12, 9);
            this.labelTS2.Name = "labelTS2";
            this.labelTS2.Size = new Size(313, 34);
            this.labelTS2.TabIndex = 4;
            this.labelTS2.Text = "PowerSDR -- Thetis --";
            this.labelTS2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTS3
            // 
            this.labelTS3.AutoSize = true;
            this.labelTS3.BackColor = System.Drawing.Color.Transparent;
            this.labelTS3.Font = new Font("Arial", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTS3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTS3.Image = null;
            this.labelTS3.Location = new Point(15, 61);
            this.labelTS3.Name = "labelTS3";
            this.labelTS3.Size = new Size(125, 14);
            this.labelTS3.TabIndex = 5;
            this.labelTS3.Text = "Special \'9+30dB Edition\'";
            this.labelTS3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTS4
            // 
            this.labelTS4.AutoSize = true;
            this.labelTS4.BackColor = System.Drawing.Color.Transparent;
            this.labelTS4.Font = new Font("Arial", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTS4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTS4.Image = null;
            this.labelTS4.Location = new Point(541, 329);
            this.labelTS4.Name = "labelTS4";
            this.labelTS4.Size = new Size(112, 14);
            this.labelTS4.TabIndex = 6;
            this.labelTS4.Text = "Also for ANAN radios";
            this.labelTS4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTS5
            // 
            this.labelTS5.Anchor = ((AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTS5.AutoSize = true;
            this.labelTS5.BackColor = System.Drawing.Color.Transparent;
            this.labelTS5.Font = new Font("Arial", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTS5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTS5.Image = null;
            this.labelTS5.Location = new Point(506, 9);
            this.labelTS5.Name = "labelTS5";
            this.labelTS5.Size = new Size(147, 14);
            this.labelTS5.TabIndex = 7;
            this.labelTS5.Text = "Our users say: \"It\'s all good\"";
            this.labelTS5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Splash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.BackgroundImage = global::Thetis.Properties.Resources.HL2Front;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new Size(677, 416);
            this.Controls.Add(this.labelTS5);
            this.Controls.Add(this.labelTS4);
            this.Controls.Add(this.labelTS3);
            this.Controls.Add(this.labelTS2);
            this.Controls.Add(this.labelTS1);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.lblStatus);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thetis Starting ...";
            this.Load += new EventHandler(this.Splash_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
#endregion

#region Static Methods
    // ************* Static Methods *************** //

    // A static method to create the thread and
    // launch the SplashScreen.
    static public void ShowSplashScreen() {
            // Make sure it is only launched once.
            if (ms_frmSplash != null)
            {
                ShowAgain();
                return;
            }
        ms_oThread = new Thread(new ThreadStart(
            ShowForm)) { IsBackground = true, Name = "Splash Screen Thread" };
        ms_oThread.Start();
    }

        static public Form form()
        {
            return ms_frmSplash;
        }

        static public void ShowAgain()
        {
            if (ms_frmSplash != null)
                ms_frmSplash.Show();
        }

    // A property returning the splash screen instance
    static public Splash SplashForm {
        get { return ms_frmSplash; }
    }

    // A private entry point for the thread.
    static private void ShowForm() {
        ms_frmSplash = new Splash();
        Control.CheckForIllegalCrossThreadCalls = false;
        Application.Run(ms_frmSplash);
    }

    // A static method to close the SplashScreen
    static public void CloseForm() {
        if (ms_frmSplash != null && ms_frmSplash.IsDisposed == false) {
            // Make it start going away.
            ms_frmSplash.m_dblOpacityIncrement
                = -ms_frmSplash.m_dblOpacityDecrement;
        }
        ms_oThread = null; // we do not need these any more.
        ms_frmSplash = null;
    }

    static public void HideForm() {
        if (ms_frmSplash != null && ms_frmSplash.IsDisposed == false)
            ms_frmSplash.Hide();
    }

    static public void UnHideForm() {
        if (ms_frmSplash != null && ms_frmSplash.IsDisposed == false)
            ms_frmSplash.Show();
    }

    // A static method to set the status and update the reference.
    static public void SetStatus(string newStatus) {
        
        SetStatus(newStatus, true);
    }

    // A static method to set the status and optionally update the reference.
    // This is useful if you are in a section of code that has a variable
    // set of status string updates.  In that case, don't set the reference.
    static public void SetStatus(string newStatus, bool setReference) {
        if (ms_frmSplash == null) return;
        ms_frmSplash.m_sStatus = newStatus;
        if (setReference) ms_frmSplash.SetReferenceInternal();
    }

    // Static method called from the initializing application to
    // give the splash screen reference points.  Not needed if
    // you are using a lot of status strings.
    static public void SetReferencePoint() {
        if (ms_frmSplash == null) return;
        ms_frmSplash.SetReferenceInternal();
    }

    // ************ Private methods ************

    // Internal method for setting reference points.
    private void SetReferenceInternal() {
        if (m_bDTSet == false) {
            m_bDTSet = true;
            m_dtStart = DateTime.Now;
            ReadIncrements();
        }
        double dblMilliseconds = ElapsedMilliSeconds();
        m_alActualTimes.Add(dblMilliseconds);
        m_dblLastCompletionFraction = m_dblCompletionFraction;
        if (m_alPreviousCompletionFraction != null
            && m_iIndex < m_alPreviousCompletionFraction.Count)
            m_dblCompletionFraction
                = (double)m_alPreviousCompletionFraction[m_iIndex++];
        else
            m_dblCompletionFraction = (m_iIndex > 0) ? 1 : 0;
    }

    // Utility function to return elapsed Milliseconds since the
    // SplashScreen was launched.
    private double ElapsedMilliSeconds() {
        TimeSpan ts = DateTime.Now - m_dtStart;
        return ts.TotalMilliseconds;
    }

    // Function to read the checkpoint intervals
    // from the previous invocation of the
    // splashscreen from the registry.
    private void ReadIncrements() {
        string sPBIncrementPerTimerInterval
            = RegistryAccess.GetStringRegistryValue(
                REGVALUE_PB_MILISECOND_INCREMENT, "0.0015");

            if (Double.TryParse(sPBIncrementPerTimerInterval,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.NumberFormatInfo.InvariantInfo,
                    out double dblResult))
                m_dblPBIncrementPerTimerInterval = dblResult;
            else
                m_dblPBIncrementPerTimerInterval = .0015;

            string sPBPreviousPctComplete
            = RegistryAccess.GetStringRegistryValue(REGVALUE_PB_PERCENTS, "");

        if (sPBPreviousPctComplete != "") {
            string[] aTimes = sPBPreviousPctComplete.Split(null);
            m_alPreviousCompletionFraction = new ArrayList();

            for (int i = 0; i < aTimes.Length; i++) {
                    if (Double.TryParse(aTimes[i],
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.NumberFormatInfo.InvariantInfo,
                            out double dblVal))
                        m_alPreviousCompletionFraction.Add(dblVal);
                    else
                        m_alPreviousCompletionFraction.Add(1.0);
                }
        } else {
            m_bFirstLaunch = true;
            lblTimeRemaining.Text = "";
        }
    }

    // Method to store the intervals (in percent complete)
    // from the current invocation of
    // the splash screen to the registry.
    private void StoreIncrements() {
        string sPercent = "";
        double dblElapsedMilliseconds = ElapsedMilliSeconds();
        for (int i = 0; i < m_alActualTimes.Count; i++)
            sPercent
                += ((double)m_alActualTimes[i] / dblElapsedMilliseconds)
                       .ToString("0.####",
                           System.Globalization.NumberFormatInfo.InvariantInfo)
                + " ";

        RegistryAccess.SetStringRegistryValue(REGVALUE_PB_PERCENTS, sPercent);

        m_dblPBIncrementPerTimerInterval = 1.0 / (double)m_iActualTicks;
        RegistryAccess.SetStringRegistryValue(REGVALUE_PB_MILISECOND_INCREMENT,
            m_dblPBIncrementPerTimerInterval.ToString("#.000000",
                System.Globalization.NumberFormatInfo.InvariantInfo));
    }

#endregion

#region Event Handlers

    //********* Event Handlers ************

    // Tick Event handler for the Timer control.
    // Handle fade in and fade out.  Also
    // handle the smoothed progress bar.
    private void timer1_Tick(object sender, EventArgs e) {
        lblStatus.Text = m_sStatus;

        if (m_dblOpacityIncrement > 0) {
            m_iActualTicks++;
            if (this.Opacity < 1) this.Opacity += m_dblOpacityIncrement;
        } else {
            if (this.Opacity > 0)
                this.Opacity += m_dblOpacityIncrement;
            else {
                StoreIncrements();
                this.Close();

                // MW0LGE interesting, but removed
                //// for -autostart option we run a task here and wait for CPU
                /// usage < 75%
                // TimeSpan ttime;
                // TimeSpan ttimeDiff;
                // var t = Task.Run(async delegate
                //{
                //     Process process = Process.GetCurrentProcess();
                //     do
                //     {
                //         ttime = process.TotalProcessorTime;
                //         await Task.Delay(500);
                //         TimeSpan ttime2 = process.TotalProcessorTime;
                //         ttimeDiff = ttime2.Subtract(ttime);
                //     } while (ttimeDiff.TotalMilliseconds > 400);
                //     Console.setPowerOn();
                // });
            }
        }
        if (m_bFirstLaunch == false
            && m_dblLastCompletionFraction < m_dblCompletionFraction) {
            m_dblLastCompletionFraction += m_dblPBIncrementPerTimerInterval;
            int width = (int)Math.Floor(
                pnlStatus.ClientRectangle.Width * m_dblLastCompletionFraction);
            int height = pnlStatus.ClientRectangle.Height;
            int x = pnlStatus.ClientRectangle.X;
            int y = pnlStatus.ClientRectangle.Y;
            if (width > 0 && height > 0) {
                m_rProgress = new Rectangle(x, y, width, height);
                // MW0LGE pnlStatus.Invalidate(m_rProgress);
                //  this call supresses background draw, which was causing the
                //  flicker annoying that c# invalidate does not have this
                //  option.
                if (pnlStatus != null && !pnlStatus.IsDisposed)
                    InvalidateRect(pnlStatus.Handle, IntPtr.Zero, false);

                int iSecondsLeft = 1
                    + (int)(TIMER_INTERVAL
                          * ((1.0 - m_dblLastCompletionFraction)
                              / m_dblPBIncrementPerTimerInterval))
                        / 1000;
                if (iSecondsLeft == 1)
                    lblTimeRemaining.Text = string.Format("1 second");
                else
                    lblTimeRemaining.Text
                        = string.Format("{0} seconds", iSecondsLeft);
            }
        }
    }

    // Paint the portion of the panel invalidated during the tick event.
    private void pnlStatus_Paint(
        object sender, PaintEventArgs e) {
            try
            {
                {
                    if (m_bFirstLaunch == false && e.ClipRectangle.Width > 0
                        && m_iActualTicks > 1 && Visible)
                    {
                        LinearGradientBrush brBackground
                            = new LinearGradientBrush(m_rProgress, Color.FromArgb(0, 80, 0),
                                Color.FromArgb(60, 180, 60), LinearGradientMode.Horizontal);
                        e.Graphics.FillRectangle(brBackground, m_rProgress);
                    }
                }
            }catch (Exception)
            {

            }
    }

#endregion

    private void Splash_Load(object sender, EventArgs e) {}
}

#region Registry Access Class

/// A class for managing registry access.
public class RegistryAccess {
    private const string SOFTWARE_KEY = "Software";
    private const string COMPANY_NAME = "OpenHPSDR";
    private const string APPLICATION_NAME = "Thetis";

    // Method for retrieving a Registry Value.
    static public string GetStringRegistryValue(
        string key, string defaultValue) {
        RegistryKey rkCompany;
        RegistryKey rkApplication;

        rkCompany = Registry.CurrentUser.OpenSubKey(SOFTWARE_KEY, false)
                        .OpenSubKey(COMPANY_NAME, false);
        if (rkCompany != null) {
            rkApplication = rkCompany.OpenSubKey(APPLICATION_NAME, true);
            if (rkApplication != null) {
                foreach (string sKey in rkApplication.GetValueNames()) {
                    if (sKey == key) {
                        return (string)rkApplication.GetValue(sKey);
                    }
                }
            }
        }
        return defaultValue;
    }

    // Method for storing a Registry Value.
    static public void SetStringRegistryValue(string key, string stringValue) {
        RegistryKey rkSoftware;
        RegistryKey rkCompany;
        RegistryKey rkApplication;

        rkSoftware = Registry.CurrentUser.OpenSubKey(SOFTWARE_KEY, true);
        rkCompany = rkSoftware.CreateSubKey(COMPANY_NAME);
        if (rkCompany != null) {
            rkApplication = rkCompany.CreateSubKey(APPLICATION_NAME);
            if (rkApplication != null) {
                rkApplication.SetValue(key, stringValue);
            }
        }
    }
}

#endregion
}
