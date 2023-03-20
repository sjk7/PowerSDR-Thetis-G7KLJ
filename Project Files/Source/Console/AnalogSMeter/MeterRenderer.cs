/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 03/04/2008
 * Ora: 14.34
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using LBSoft.IndustrialCtrls.Utils;
using System.IO;

namespace LBSoft.IndustrialCtrls.Meters {
  /// <summary>
  /// Base class for the renderers of the analog meter
  /// </summary>
  public class LBAnalogMeterRenderer {
#region Variables
    /// <summary>
    /// Control to render
    /// </summary>
    private LBAnalogMeter meter = null;

    protected Bitmap m_Img = Thetis.Properties.Resources.NewVFOAnalogSignalGauge;

    public Image BackGroundCustomImage {
      get => m_Img;
      set {
        // converting background image to particular format, which is meant to be
        // the most efficient in GDI+
        m_Img = new Bitmap(value.Width, value.Height,
                           System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

        using (Graphics gr = Graphics.FromImage(m_Img)) {
          gr.DrawImage(value, new Rectangle(0, 0, m_Img.Width, m_Img.Height));
        }
      }
    }
#endregion

#region Properies
    public LBAnalogMeter AnalogMeter {
      set { this.meter = value; }
      get { return this.meter; }
    }
#endregion

#region Virtual method
    /// <summary>
    /// Draw the background of the control
    /// </summary>
    /// <param name="gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawBackground(Graphics gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the body of the control
    /// </summary>
    /// <param name="Gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawBody(Graphics Gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the scale of the control
    /// </summary>
    /// <param name="Gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawDivisions(Graphics Gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the thresholds
    /// </summary>
    /// <param name="gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawThresholds(Graphics gr, RectangleF rc) { return false; }

    /// <summary>
    /// Drawt the unit measure of the control
    /// </summary>
    /// <param name="gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawUM(Graphics gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the current value in numerical form
    /// </summary>
    /// <param name="gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawValue(Graphics gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the needle
    /// </summary>
    /// <param name="Gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawNeedle(Graphics Gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the needle cover at the center
    /// </summary>
    /// <param name="Gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawNeedleCover(Graphics Gr, RectangleF rc) { return false; }

    /// <summary>
    /// Draw the glass effect
    /// </summary>
    /// <param name="Gr"></param>
    /// <param name="rc"></param>
    /// <returns></returns>
    public virtual bool DrawGlass(Graphics Gr, RectangleF rc) { return false; }
#endregion
  }

  /// <summary>
  /// NewVFOAnalogSignalGauge renderer class for the analog meter
  /// </summary>
  public class LBDefaultAnalogMeterRenderer : LBAnalogMeterRenderer {

    public override bool DrawBackground(Graphics gr, RectangleF rc) {
      if (this.AnalogMeter == null)
        return false;

      gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
      // gr.SmoothingMode = SmoothingMode.HighSpeed;
      // gr.PixelOffsetMode = PixelOffsetMode.HighSpeed;
      // gr.CompositingQuality = CompositingQuality.HighSpeed;

      gr.DrawImage(m_Img, AnalogMeter.DisplayRectangle);

      return true;
    }

    public override bool DrawBody(Graphics Gr, RectangleF rc) {
      if (this.AnalogMeter == null)
        return false;
      return false;
      /*/
Color bodyColor = this.AnalogMeter.BodyColor;
Color cDark = LBColorManager.StepColor(bodyColor, 20);

LinearGradientBrush br1 = new LinearGradientBrush(rc, bodyColor, cDark, 45);
Gr.FillEllipse(br1, rc);

float drawRatio = this.AnalogMeter.GetDrawRatio();

RectangleF _rc = rc;
_rc.X += 3 * drawRatio;
_rc.Y += 3 * drawRatio;
_rc.Width -= 6 * drawRatio;
_rc.Height -= 6 * drawRatio;

LinearGradientBrush br2 = new LinearGradientBrush(_rc, cDark, bodyColor, 45);
Gr.FillEllipse(br2, _rc);

return true;
      /*/
    }

    public override bool DrawThresholds(Graphics gr, RectangleF rc) { return false; }

    public override bool DrawDivisions(Graphics Gr, RectangleF rc) {
      if (this.AnalogMeter == null)
        return false;
      return false;
      /*/
PointF needleCenter = this.AnalogMeter.GetNeedleCenter();
float startAngle = this.AnalogMeter.GetStartAngle();
float endAngle = this.AnalogMeter.GetEndAngle();
float scaleDivisions = this.AnalogMeter.ScaleDivisions;
float scaleSubDivisions = this.AnalogMeter.ScaleSubDivisions;
float drawRatio = this.AnalogMeter.GetDrawRatio();
double minValue = this.AnalogMeter.MinValue;
double maxValue = this.AnalogMeter.MaxValue;
Color scaleColor = this.AnalogMeter.ScaleColor;

float cx = needleCenter.X;
float cy = needleCenter.Y;
float w = rc.Width;
float h = rc.Height;

float incr = LBMath.GetRadian((endAngle - startAngle) /
                              ((scaleDivisions - 1) * (scaleSubDivisions + 1)));
float currentAngle = LBMath.GetRadian(startAngle);
float radius = (float)(w / 2 - (w * 0.08));
float rulerValue = (float)minValue;

Pen pen = new Pen(scaleColor, (2 * drawRatio));
SolidBrush br = new SolidBrush(scaleColor);

PointF ptStart = new PointF(0, 0);
PointF ptEnd = new PointF(0, 0);
int n = 0;
for (; n < scaleDivisions; n++) {
  // Draw Thick Line
  ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
  ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));
  ptEnd.X = (float)(cx + (radius - w / 20) * Math.Cos(currentAngle));
  ptEnd.Y = (float)(cy + (radius - w / 20) * Math.Sin(currentAngle));
  Gr.DrawLine(pen, ptStart, ptEnd);

  // Draw Strings
  Font font = new Font(this.AnalogMeter.Font.FontFamily, (float)(6F * drawRatio));

  float tx = (float)(cx + (radius - (20 * drawRatio)) * Math.Cos(currentAngle));
  float ty = (float)(cy + (radius - (20 * drawRatio)) * Math.Sin(currentAngle));
  double val = Math.Round(rulerValue);
  String str = String.Format("{0,0:D}", (int)val);

  SizeF size = Gr.MeasureString(str, font);
  Gr.DrawString(str, font, br, tx - (float)(size.Width * 0.5),
                ty - (float)(size.Height * 0.5));

  rulerValue += (float)((maxValue - minValue) / (scaleDivisions - 1));

  if (n == scaleDivisions - 1)
    break;

  if (scaleDivisions <= 0)
    currentAngle += incr;
  else {
    for (int j = 0; j <= scaleSubDivisions; j++) {
      currentAngle += incr;
      ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
      ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));
      ptEnd.X = (float)(cx + (radius - w / 50) * Math.Cos(currentAngle));
      ptEnd.Y = (float)(cy + (radius - w / 50) * Math.Sin(currentAngle));
      Gr.DrawLine(pen, ptStart, ptEnd);
    }
  }
}

return true;
      /*/
    }

    public override bool DrawUM(Graphics gr, RectangleF rc) { return false; }

    public override bool DrawValue(Graphics gr, RectangleF rc) { return false; }

    public override bool DrawNeedle(Graphics Gr, RectangleF rc) {
      if (this.AnalogMeter == null)
        return false;

      float w, h;
      w = rc.Width;
      h = rc.Height;

      double minValue = this.AnalogMeter.MinValue;
      double maxValue = this.AnalogMeter.MaxValue;
      double currValue = this.AnalogMeter.Value;
      float startAngle = this.AnalogMeter.GetStartAngle();
      float endAngle = this.AnalogMeter.GetEndAngle();
      PointF needleCenter = this.AnalogMeter.GetNeedleCenter();

      float radius = (float)(w / 2 - (w * 0.12));
      float val = (float)(maxValue - minValue);

      val = (float)((100 * (currValue - minValue)) / val);
      val = ((endAngle - startAngle) * val) / 100;
      val += startAngle;

      float angle = LBMath.GetRadian(val);

      float cx = needleCenter.X;
      float cy = needleCenter.Y;

      PointF ptStart = new PointF(0, 0);
      PointF ptEnd = new PointF(0, 0);

      GraphicsPath pth1 = new GraphicsPath();

      ptStart.X = cx;
      ptStart.Y = cy;
      angle = LBMath.GetRadian(val + 10);
      // this controls needle width
      ptEnd.X = (float)(cx + (w * .01F) * Math.Cos(angle));
      ptEnd.Y = (float)(cy + (w * .01F) * Math.Sin(angle));
      pth1.AddLine(ptStart, ptEnd);

      ptStart = ptEnd;
      angle = LBMath.GetRadian(val);
      ptEnd.X = (float)(cx + (radius * 1.5f) * Math.Cos(angle));
      ptEnd.Y = (float)(cy + (radius * 1.5f) * Math.Sin(angle));

      pth1.AddLine(ptStart, ptEnd);

      /*/
  ptStart = ptEnd;
  angle = LBMath.GetRadian(val - 10);
      ptEnd.X = (float)(cx + (w * .09F) * Math.Cos(angle));
  ptEnd.Y = (float)(cy + (w * .09F) * Math.Sin(angle));
  pth1.AddLine ( ptStart, ptEnd );

  pth1.CloseFigure();
  /*/

      SolidBrush br = new SolidBrush(Color.Red);

      // Pen pen = new Pen ( this.AnalogMeter.NeedleColor );
      Pen pen = new Pen(Color.Red);
      pen.Width = 2;
      Gr.DrawPath(pen, pth1);
      Gr.FillPath(br, pth1);

      return true;
    }

    public override bool DrawNeedleCover(Graphics Gr, RectangleF rc) {
      if (this.AnalogMeter == null)
        return false;

      Color clr = this.AnalogMeter.NeedleColor;
      RectangleF _rc = rc;
      float drawRatio = this.AnalogMeter.GetDrawRatio();

      Color clr1 = Color.FromArgb(70, clr);

      _rc.Inflate(5 * drawRatio, 5 * drawRatio);

      SolidBrush brTransp = new SolidBrush(clr1);
      Gr.FillEllipse(brTransp, _rc);

      clr1 = clr;
      Color clr2 = LBColorManager.StepColor(clr, 75);
      LinearGradientBrush br1 = new LinearGradientBrush(rc, clr1, clr2, 45);
      Gr.FillEllipse(br1, rc);
      return true;
    }

    public override bool DrawGlass(Graphics Gr, RectangleF rc) {
      if (this.AnalogMeter == null)
        return false;

      if (this.AnalogMeter.ViewGlass == false)
        return true;

      Color clr1 = Color.FromArgb(40, 200, 200, 200);

      Color clr2 = Color.FromArgb(0, 200, 200, 200);
      LinearGradientBrush br1 = new LinearGradientBrush(rc, clr1, clr2, 45);
      Gr.FillEllipse(br1, rc);

      return true;
    }
  }
}
