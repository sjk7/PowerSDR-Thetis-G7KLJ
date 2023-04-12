/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 03/04/2008
 * Ora: 14.34
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LBSoft.IndustrialCtrls.Utils;
using Thetis.Properties;

namespace LBSoft.IndustrialCtrls.Meters {
/// <summary>
/// Class for the analog meter control
/// </summary>
public partial class LBAnalogMeter : UserControl {
    public event EventHandler ValueChanged;
    public event EventHandler MaxValueChanged;
    public event EventHandler MinValueChanged;
    public event EventHandler BackGndImgChanged;

    public class BackGndChanged : EventArgs {
        public int which = 0;
    }
#region Enumerator
    public enum AnalogMeterStyle {
        Circular = 0,
    }
    ;
#endregion

#region Properties variables
    private AnalogMeterStyle meterStyle;
    private Color bodyColor;
    private Color needleColor;
    private Color scaleColor;
    private bool viewGlass;
    private double currValue;
    private double minValue;
    private double maxValue;
    private int scaleDivisions;
    private int scaleSubDivisions;
    private LBAnalogMeterRenderer renderer;
#endregion

#region Class variables
    protected PointF needleCenter;
    protected RectangleF drawRect;
    protected RectangleF glossyRect;
    protected RectangleF needleCoverRect;
    protected float startAngle;
    protected float endAngle;
    protected float drawRatio;
    private ContextMenuStrip mnuBigSMeter;
    private IContainer components;
    private ToolStripMenuItem chooseBackgroundImageToolStripMenuItem;
    private ToolStripMenuItem version1ToolStripMenuItem;
    private ToolStripMenuItem version2ToolStripMenuItem;
    protected LBAnalogMeterRenderer defaultRenderer;
#endregion
    // choices below zero are temporary (likely only used in MOX, so we don't
    // save them)
    public enum BackGroundChoices {
        VU = -3,
        PPM = -2,
        Skin = 0,
        NewVFOAnalogSignalGauge = 1,
        Blue = 2,
        Tango = 3,
        Kenwood = 4,
        VKK = 5
    }

    BackGroundChoices m_backGroundChoice;
    public BackGroundChoices CurrentBackGroundChoice() {
        return m_backGroundChoice;
    }

#region Constructors
    public LBAnalogMeter() {
        // Initialization
        InitializeComponent();

        // Properties initialization
        this.bodyColor = Color.Red;
        this.needleColor = Color.Yellow;
        this.scaleColor = Color.White;
        this.meterStyle = AnalogMeterStyle.Circular;
        this.viewGlass = Settings.Default.SMeterGlass;
        this.startAngle = 225;
        this.endAngle = 315;
        this.minValue = 0;
        this.maxValue = 1000;
        this.currValue = 0;
        this.scaleDivisions = 10;
        this.scaleSubDivisions = 10;
        this.renderer = null;

        // Set the styles for drawing
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        this.SetStyle(ControlStyles.UserPaint, true);

        // Create the default renderer
        this.defaultRenderer
            = new LBDefaultAnalogMeterRenderer { AnalogMeter = this };
    }
#endregion

#region Properties
    [Category("Appearance"), Description("Style of the control")]
    public AnalogMeterStyle MeterStyle {
        get { return meterStyle; }
        set {
            meterStyle = value;
            Invalidate();
        }
    }

    [Category("Appearance"), Description("Color of the body of the control")]
    public Color BodyColor {
        get { return bodyColor; }
        set {
            bodyColor = value;
            Invalidate();
        }
    }

    [Category("Appearance"), Description("Color of the needle")]
    public Color NeedleColor {
        get { return needleColor; }
        set {
            needleColor = value;
            Invalidate();
        }
    }

    [Category("Appearance"), Description("Show or hide the glass effect")]
    public bool ViewGlass {
        get { return viewGlass; }
        set {
            viewGlass = value;
            Invalidate();
            BackGndChanged e = new BackGndChanged { which
                = Settings.Default.SMeterBackgroundImg };
            if (this.InvokeRequired) {
                this.BackGndImgChanged?.Invoke(this, e);
            } else {
                if (this.BackGndImgChanged != null) {
                    this.BackGndImgChanged(this, e);
                }
            }
            Settings.Default.SMeterGlass = value;
            Settings.Default.Save();
        }
    }

    [Category("Appearance"), Description("Color of the scale of the control")]
    public Color ScaleColor {
        get { return scaleColor; }
        set {
            scaleColor = value;
            Invalidate();
        }
    }

    public class ValueEvent : EventArgs {
        public float currentVal = 0.0f;
    }

    [Category("Behavior"), Description("Value of the data")]
    public double Value {
        get { return currValue; }
        set {

            double val = value;
            if (val > maxValue) val = maxValue;

            if (val < minValue) val = minValue;

            ValueEvent e = new ValueEvent { currentVal = (float)val };
            currValue = val;
            Invalidate();
            this.ValueChanged?.Invoke(this, e);
        }
    }

    ValueEvent dummy_event_data = new ValueEvent();
    public double Max {
        get { return maxValue; }
        set {
            maxValue = value;
            this.MaxValueChanged?.Invoke(this, dummy_event_data);
            Invalidate();
        }
    }

    public void setValue(float newVal) { Value = newVal; }

    [Category("Behavior"), Description("Minimum value of the data")]
    public double MinValue {
        get { return minValue; }
        set {
            var oldValue = minValue;

            minValue = value;
            this.MinValueChanged?.Invoke(this, dummy_event_data);
            if (minValue != oldValue) Invalidate();
        }
    }

    [Category("Behavior"), Description("Maximum value of the data")]
    public double MaxValue {
        get { return maxValue; }
        set {
            var oldMax = maxValue;
            maxValue = value;
            this.MaxValueChanged?.Invoke(this, dummy_event_data);
            if (oldMax != value) {
                Invalidate();
            }
        }
    }

    [Category("Appearance"), Description("Number of the scale divisions")]
    public int ScaleDivisions {
        get { return scaleDivisions; }
        set {
            scaleDivisions = value;
            CalculateDimensions();
            Invalidate();
        }
    }

    [Category("Appearance"), Description("Number of the scale subdivisions")]
    public int ScaleSubDivisions {
        get { return scaleSubDivisions; }
        set {
            scaleSubDivisions = value;
            CalculateDimensions();
            Invalidate();
        }
    }

    [Browsable(false)]
    public LBAnalogMeterRenderer Renderer {
        get { return this.renderer; }
        set {
            this.renderer = value;
            if (this.renderer != null) renderer.AnalogMeter = this;
            Invalidate();
        }
    }

#endregion

#region Public methods
    public float GetDrawRatio() { return this.drawRatio; }

    public float GetStartAngle() { return this.startAngle; }

    public float GetEndAngle() { return this.endAngle; }

    public PointF GetNeedleCenter() { return this.needleCenter; }
#endregion

#region Events delegates
    protected override void OnSizeChanged(EventArgs e) {
        base.OnSizeChanged(e);

        // Calculate dimensions
        CalculateDimensions();

        this.Invalidate();
    }

    protected override void OnPaintBackground(PaintEventArgs e) {}

    protected override void OnPaint(PaintEventArgs e) {
        RectangleF _rc = new RectangleF(0, 0, this.Width, this.Height);
        e.Graphics.SmoothingMode
            = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        if (this.renderer == null) {
            this.defaultRenderer.DrawBackground(e.Graphics, _rc);
            this.defaultRenderer.DrawBody(e.Graphics, drawRect);
            this.defaultRenderer.DrawThresholds(e.Graphics, drawRect);
            this.defaultRenderer.DrawDivisions(e.Graphics, drawRect);
            this.defaultRenderer.DrawUM(e.Graphics, drawRect);
            this.defaultRenderer.DrawValue(e.Graphics, drawRect);
            this.defaultRenderer.DrawNeedle(e.Graphics, drawRect);
            this.defaultRenderer.DrawNeedleCover(
                e.Graphics, this.needleCoverRect);
            this.defaultRenderer.DrawGlass(e.Graphics, this.glossyRect);
        } else {
            if (this.Renderer.DrawBackground(e.Graphics, _rc) == false)
                this.defaultRenderer.DrawBackground(e.Graphics, _rc);
            if (this.Renderer.DrawBody(e.Graphics, drawRect) == false)
                this.defaultRenderer.DrawBody(e.Graphics, drawRect);
            if (this.Renderer.DrawThresholds(e.Graphics, drawRect) == false)
                this.defaultRenderer.DrawThresholds(e.Graphics, drawRect);
            if (this.Renderer.DrawDivisions(e.Graphics, drawRect) == false)
                this.defaultRenderer.DrawDivisions(e.Graphics, drawRect);
            if (this.Renderer.DrawUM(e.Graphics, drawRect) == false)
                this.defaultRenderer.DrawUM(e.Graphics, drawRect);
            if (this.Renderer.DrawValue(e.Graphics, drawRect) == false)
                this.defaultRenderer.DrawValue(e.Graphics, drawRect);
            if (this.Renderer.DrawNeedle(e.Graphics, drawRect) == false)
                this.defaultRenderer.DrawNeedle(e.Graphics, drawRect);
            if (this.Renderer.DrawNeedleCover(e.Graphics, this.needleCoverRect)
                == false)
                this.defaultRenderer.DrawNeedleCover(
                    e.Graphics, this.needleCoverRect);
            if (this.Renderer.DrawGlass(e.Graphics, this.glossyRect) == false)
                this.defaultRenderer.DrawGlass(e.Graphics, this.glossyRect);
        }
    }
#endregion

#region Virtual functions
    protected virtual void CalculateDimensions() {
        // Rectangle
        float x, y, w, h;
        x = 0;
        y = 0;
        w = this.Size.Width;
        h = this.Size.Height;

        // Calculate ratio
        drawRatio = (Math.Min(w, h)) / 200;
        if (drawRatio == 0.0) drawRatio = 1;

        // Draw rectangle
        drawRect.X = x;
        drawRect.Y = y;
        drawRect.Width = w - 2;
        drawRect.Height = h - 2;

        // if ( w < h )
        //	drawRect.Height = w;
        // else if ( w > h )
        //	drawRect.Width = h;

        if (drawRect.Width < 10) drawRect.Width = 10;
        if (drawRect.Height < 10) drawRect.Height = 10;

        // Calculate needle center
        // needleCenter.X = drawRect.X + ( drawRect.Width / 2 );
        // needleCenter.Y =  drawRect.Y + ( drawRect.Height / 2 );

        needleCenter.X = drawRect.X + (drawRect.Width / 2);
        needleCenter.Y = drawRect.Height + (15 * drawRatio);

        // Needle cover rect
        var cover = 45;
        needleCoverRect.X = needleCenter.X - (cover / 2 * drawRatio);
        needleCoverRect.Y = needleCenter.Y - (cover / 2 * drawRatio);
        needleCoverRect.Width = cover * drawRatio;
        needleCoverRect.Height = cover * drawRatio;

        // Glass effect rect
        glossyRect.X = drawRect.X + (20 * drawRatio);
        glossyRect.Y = drawRect.Y + (20 * drawRatio);
        glossyRect.Width = drawRect.Width - (30 * drawRatio);
        glossyRect.Height = needleCenter.Y + (30 * drawRatio);
    }
#endregion

    private void InitializeComponent() {
        this.components = new Container();
        this.mnuBigSMeter = new ContextMenuStrip(this.components);
        this.chooseBackgroundImageToolStripMenuItem = new ToolStripMenuItem();
        this.version1ToolStripMenuItem = new ToolStripMenuItem();
        this.version2ToolStripMenuItem = new ToolStripMenuItem();
        this.mnuBigSMeter.SuspendLayout();
        this.SuspendLayout();
        //
        // mnuBigSMeter
        //
        this.mnuBigSMeter.Items.AddRange(new ToolStripItem[] {
            this.chooseBackgroundImageToolStripMenuItem
        });
        this.mnuBigSMeter.Name = "mnuBigSMeter";
        this.mnuBigSMeter.Size = new Size(218, 48);
        //
        // chooseBackgroundImageToolStripMenuItem
        //

        //
        // LBAnalogMeter
        //
        this.Name = "LBAnalogMeter";
        this.mnuBigSMeter.ResumeLayout(false);
        this.viewGlass = Settings.Default.SMeterGlass;
        this.ResumeLayout(false);
    }
    public Thetis.Console m_console;

    private Image m_bio;
    public override Image BackgroundImage {
        get { return m_bio; }
        set {
            // converting background image to particular format, which is meant
            // to be the most efficient in GDI+
            Bitmap bmp = new Bitmap(value.Width, value.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            using (Graphics gr = Graphics.FromImage(bmp)) {
                gr.DrawImage(
                    value, new Rectangle(0, 0, this.Width, this.Height));
            }
            m_bio = value;
            if (defaultRenderer != null) {
                defaultRenderer.BackGroundCustomImage = m_bio;
                if (m_console != null) {
                    m_console.PrettySMeter.defaultRenderer.BackGroundCustomImage
                        = renderer.BackGroundCustomImage;
                }
            }
        }
    }

    public void SaveBackGroundImage() {}

    public void ToggleBackGroundImage(
        BackGroundChoices which = BackGroundChoices.Blue) {

        int cur = Settings.Default.SMeterBackgroundImg;
        var renderer = Renderer;
        bool force = false;
        if (renderer == null) {
            force = true;
            Renderer = this.defaultRenderer;
            renderer = Renderer;
        }
        if (m_console != null) {
            Image skinpic = m_console.PrettySMeterSkin();
            if (skinpic != null) {
                renderer.BackGroundCustomImage = skinpic;
                m_backGroundChoice = BackGroundChoices.Skin;
                goto done;
            }
        }
        if (which == BackGroundChoices.Skin) {
            // here, Skin is being requested, but we know from the lines above
            // that there _is_ no skin pic available
            which = BackGroundChoices.NewVFOAnalogSignalGauge;
        }

        if (which == m_backGroundChoice && !force) {
            goto done;
        }

        switch (which) {
            case BackGroundChoices.Blue:
                renderer.BackGroundCustomImage
                    = Thetis.Properties.Resources.OLDAnalogSignalGauge;
                break;

            case BackGroundChoices.PPM:
                renderer.BackGroundCustomImage
                    = Thetis.Properties.Resources.PPM;
                break;

            case BackGroundChoices.NewVFOAnalogSignalGauge:
                renderer.BackGroundCustomImage
                    = Thetis.Properties.Resources.NewVFOAnalogSignalGauge;
                break;

            case BackGroundChoices.Tango:
                renderer.BackGroundCustomImage
                    = Thetis.Properties.Resources.SMeterTango;
                break;

            case BackGroundChoices.VU:
                renderer.BackGroundCustomImage = Thetis.Properties.Resources.VU;
                break;
            case BackGroundChoices.Kenwood:
                renderer.BackGroundCustomImage
                    = Thetis.Properties.Resources.Kenwood;
                break;
            case BackGroundChoices.VKK:
                renderer.BackGroundCustomImage
                    = Thetis.Properties.Resources.VKK;
                break;

            default: Debug.Assert(false); break;
        }

    done:
        m_backGroundChoice = which;
        this.ViewGlass = Settings.Default.SMeterGlass;
        // do not save something that is only for Tx, like PPM
        // Achieved here because any mox ones have which <= 0
        if (which > 0) {
            Settings.Default.SMeterBackgroundImg = (int)which;
            Settings.Default.Save();
        }

        Invalidate();
        Refresh();
        BackGndChanged e = new BackGndChanged { which
            = Settings.Default.SMeterBackgroundImg };
        if (this.InvokeRequired) {
            this.BackGndImgChanged?.Invoke(this, e);
        } else {
            if (this.BackGndImgChanged != null) {
                this.BackGndImgChanged(this, e);
            }
        }

        if (m_console != null) {
            m_console.PrettySMeter.defaultRenderer.BackGroundCustomImage
                = renderer.BackGroundCustomImage;
        }
    }
}
}
