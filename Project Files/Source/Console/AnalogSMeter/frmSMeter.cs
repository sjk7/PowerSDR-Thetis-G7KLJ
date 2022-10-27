using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Specialized;
using Thetis.Properties;

namespace Thetis {
public partial class frmSMeter : Form {
    private bool windowInitialized = false;
    private Thetis.Console m_console;

    public Thetis.Console console {
        get { return m_console; }

        set {
            m_console = value;
            this.BigSMeter.m_console = value;
        }
    }

    public frmSMeter(
        System.Drawing.Rectangle initPosition, Console the_console) {
        InitializeComponent();
        // this is the default
        console = the_console;
        this.WindowState = FormWindowState.Normal;
        this.StartPosition = FormStartPosition.WindowsDefaultBounds;
        this.Owner = the_console;

        // Properties.Settings.Default.Reset();

        // this.ControlBox = false;
        // this.Text = String.Empty;
        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        Text = string.Empty;
        ControlBox = false;
        FormBorderStyle = FormBorderStyle.SizableToolWindow;

        if (Settings.Default.WindowPosition == Rectangle.Empty) {
            initPosition.Height
                += (int)((float)initPosition.Height / (float)0.7);
            initPosition.Width *= 2;
            initPosition.X -= initPosition.Width * 2;
            Settings.Default.WindowPosition = initPosition;
        }
        // check if the saved bounds are nonzero and visible on any screen
        if (Settings.Default.WindowPosition != Rectangle.Empty
            && Common.IsVisibleOnAnyScreen(Settings.Default.WindowPosition)) {
            // first set the bounds
            this.StartPosition = FormStartPosition.Manual;
            this.DesktopBounds = Settings.Default.WindowPosition;

            // afterwards set the window state to the saved value (which could
            // be Maximized)
            this.WindowState = Settings.Default.WindowState;
        } else {
            // this resets the upper left corner of the window to windows
            // standards
            StartPosition = FormStartPosition.WindowsDefaultLocation;

            // we can still apply the saved size
            Size = Settings.Default.WindowPosition.Size;
        }
        windowInitialized = true;
        Settings.Default.BigSMeterOpen = true;
        BigSMeter.ToggleBackGroundImage(Settings.Default.SMeterBackgroundImg);

        Settings.Default.Save();
        if (Settings.Default.TopMost) this.TopMost = true;

        if (TopMost)
            this.Owner = null;
        else
            this.Owner = console;

        this.alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
    }

    protected override void OnResize(EventArgs e) {
        base.OnResize(e);
        TrackWindowState();
    }

    protected override void OnMove(EventArgs e) {
        base.OnMove(e);
        TrackWindowState();
    }

    // On a move or resize in Normal state, record the new values as they occur.
    // This solves the problem of closing the app when minimized or maximized.
    private void TrackWindowState() {
        // Don't record the window setup, otherwise we lose the persistent
        // values!
        if (!windowInitialized) {
            return;
        }

        if (WindowState == FormWindowState.Normal) {
            Settings.Default.WindowState = FormWindowState.Normal;
            Settings.Default.WindowPosition = this.DesktopBounds;
            Settings.Default.Save();
        }
    }

    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);

        // only save the WindowState if Normal or Maximized
        switch (this.WindowState) {
            case FormWindowState.Normal:

                Settings.Default.WindowState = this.WindowState;
                Settings.Default.WindowPosition = this.DesktopBounds;
                break;
            case FormWindowState.Maximized:
                Settings.Default.WindowState
                    = this.WindowState; // save the windowstate but NOT the
                                        // size!
                break;
            default:
                Settings.Default.WindowState = FormWindowState.Normal;
                break;
        }

        Settings.Default.Save();
    }

    public double Value {
        get => this.BigSMeter.Value;
        set => this.BigSMeter.Value = value;
    }

    private void BigSMeter_Load(object sender, EventArgs e) {}

    private void frmSMeter_Load(object sender, EventArgs e) {}

    private void originalToolStripMenuItem_Click(object sender, EventArgs e) {
        BigSMeter.ToggleBackGroundImage(0);
    }

    private void blueToolStripMenuItem_Click(object sender, EventArgs e) {
        BigSMeter.ToggleBackGroundImage(1);
    }

    private void youKnowWhenYouveBeenTangodToolStripMenuItem_Click(
        object sender, EventArgs e) {
        BigSMeter.ToggleBackGroundImage(2);
    }

    private void BigSMeter_BackGndImgChanged(object sender, EventArgs e) {
        if (m_console != null) {
            m_console.PrettySMeter.ToggleBackGroundImage(
                Settings.Default.SMeterBackgroundImg);
        }
    }

    private void BigSMeter_MouseClick(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Right) {
            this.mnuBigSMeter.Show(Cursor.Position);
            this.alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
            if (console.PrettySMeterSkin() != null) {
                this.originalToolStripMenuItem.Enabled = false;
                this.blueToolStripMenuItem.Enabled = false;
                this.whyCantIChooseTheBackgroundToolStripMenuItem.Enabled
                    = true;
            } else {
                this.originalToolStripMenuItem.Enabled = true;
                this.blueToolStripMenuItem.Enabled = true;
                this.whyCantIChooseTheBackgroundToolStripMenuItem.Enabled
                    = false;
            }
        }
    }
    private Point MouseDownLocation;

    private void BigSMeter_MouseDown(object sender, MouseEventArgs e) {
        if (e.Button == System.Windows.Forms.MouseButtons.Left) {
            MouseDownLocation = e.Location;
        }
    }

    private void BigSMeter_MouseMove(object sender, MouseEventArgs e) {
        if (e.Button == System.Windows.Forms.MouseButtons.Left) {
            Left = e.X + Left - MouseDownLocation.X;
            Top = e.Y + Top - MouseDownLocation.Y;
        }
    }

    private void BigSMeter_DoubleClick(object sender, EventArgs e) {
        if (this.WindowState == FormWindowState.Maximized)
            this.WindowState = FormWindowState.Normal;
        else if (this.WindowState == FormWindowState.Minimized)
            this.WindowState = FormWindowState.Normal;
        else
            this.WindowState = FormWindowState.Maximized;
    }

    int Mx;
    int My;
    int Sw;
    int Sh;
    bool mov;

    private void Grip_MouseDown(object sender, MouseEventArgs e) {
        mov = true;
        My = MousePosition.Y;
        Mx = MousePosition.X;
        Sw = Width;
        Sh = Height;
        Cursor = Cursors.Hand;
    }

    private void Grip_MouseMove(object sender, MouseEventArgs e) {
        if (mov == true) {
            Width = MousePosition.X - Mx + Sw;
            Height = MousePosition.Y - My + Sh;
        }
    }

    private void Grip_MouseEnter(object sender, EventArgs e) {}

    private void Grip_MouseLeave(object sender, EventArgs e) {}

    private void Grip_MouseUp(object sender, MouseEventArgs e) {
        mov = false;
        Cursor = Cursors.Default;
    }

    private void normalToolStripMenuItem_Click(object sender, EventArgs e) {
        this.WindowState = FormWindowState.Normal;
    }

    private void minimizedToolStripMenuItem_Click(object sender, EventArgs e) {
        this.WindowState = FormWindowState.Minimized;
    }

    private void maximizedToolStripMenuItem_Click(object sender, EventArgs e) {
        this.WindowState = FormWindowState.Maximized;
    }

    private void alwaysOnTopToolStripMenuItem_Click(
        object sender, EventArgs e) {
        Settings.Default.TopMost = !Settings.Default.TopMost;
        Settings.Default.Save();
        this.alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
        this.TopMost = Settings.Default.TopMost;
        var test = Settings.Default.TopMost;
        if (this.TopMost)
            this.Owner = null;
        else
            this.Owner = console;
    }

    private void doNotShowBigSMeterToolStripMenuItem_Click(
        object sender, EventArgs e) {
        Settings.Default.BigSMeterOpen = false;
        Settings.Default.Save();
        this.Close();
    }

    private void whyCantIChooseTheBackgroundToolStripMenuItem_Click(
        object sender, EventArgs e) {
        string msg1
            = "You can't change the background image because you have a skin installed for the Pretty S Meter background, at:";
        msg1 += "\n\n";
        msg1 += console.PrettySMeterSkinPath();
        msg1 += "\n\n";
        msg1
            += "You must first rename or move this file, then restart this application.";
        MessageBox.Show(msg1, "Pretty S-Meter Message");
    }
}
}
