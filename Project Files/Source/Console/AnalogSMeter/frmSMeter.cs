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
  public frmSMeter() {
    InitializeComponent();
    // this is the default
    this.WindowState = FormWindowState.Normal;
    this.StartPosition = FormStartPosition.WindowsDefaultBounds;

    // check if the saved bounds are nonzero and visible on any screen
    if (Settings.Default.WindowPosition != Rectangle.Empty &&
        IsVisibleOnAnyScreen(Settings.Default.WindowPosition)) {
      // first set the bounds
      this.StartPosition = FormStartPosition.Manual;
      this.DesktopBounds = Settings.Default.WindowPosition;

      // afterwards set the window state to the saved value (which could be Maximized)
      this.WindowState = Settings.Default.WindowState;
    } else {
      // this resets the upper left corner of the window to windows standards
      this.StartPosition = FormStartPosition.WindowsDefaultLocation;

      // we can still apply the saved size
      Size = Settings.Default.WindowPosition.Size;
    }
    windowInitialized = true;
    Settings.Default.BigSMeterOpen = true;
    Settings.Default.Save();
    this.TopMost = true;
  }

  private bool IsVisibleOnAnyScreen(Rectangle rect) {
    foreach (Screen screen in Screen.AllScreens) {
      if (screen.WorkingArea.IntersectsWith(rect)) {
        return true;
      }
    }

    return false;
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
    // Don't record the window setup, otherwise we lose the persistent values!
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
      Settings.Default.WindowState = this.WindowState; // save the windowstate but NOT the size!
      break;
    default:
      Settings.Default.WindowState = FormWindowState.Normal;
      break;
    }
    Settings.Default.BigSMeterOpen = false;
    Settings.Default.Save();
  }

  public double Value {
    get => this.BigSMeter.Value;
    set => this.BigSMeter.Value = value;
  }
}
}
