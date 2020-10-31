//=================================================================
// SMeter.cs
//=================================================================
//
// Copyright (C)2011-2013 YT7PWR Goran Radivojevic
// contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
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
//=================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if DirectX
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using System.Threading;
#endif

namespace Thetis.PowerSDR
{



    public class SMeter : Control
    {

        protected override void OnResize(EventArgs e)

        {
            var oldW = Width;
            base.OnResize(e);

         }
        #region variable
        // Thetis.PowerSDR.FilterButterworth lpf;

#if DirectX
        public SharpDX.Direct3D11.Device device = null;
        private Texture BackgroundTexture = null;
        private Sprite sprite = null;
        private Rectangle texture_size;
        SlimDX.Direct3D9.Line line;
        Vector2[] verts1;
        public bool DX_reinit = false;
#endif
        private delegate void DebugCallbackFunction(string name);
        public bool debug = false;
        public bool booting = false;
        private Int32 baseArcStart = 230;
        private Int32 baseArcSweep = 77;
        private Single m_value = 0;
        public Int32 m_NeedleRadius = 160;
        // NOTE: original size in GSDR was 240*133
        private Point m_Center = new Point(120, 180);
        //        private Boolean drawGaugeBackground = true;
        public Bitmap gaugeBitmap;

        #endregion

        #region properties

#if DirectX
        private RenderType directx_render_type = RenderType.HARDWARE;
        public RenderType DirectXRenderType
        {
            get { return directx_render_type; }
            set { directx_render_type = value; }
        }
#endif

        private Thetis.DisplayEngine display_engine = Thetis.DisplayEngine.GDI_PLUS;
        public Thetis.DisplayEngine displayEngine
        {
            get { return display_engine; }
            set { display_engine = value; }
        }

        private Control gauge_target = null;
        public Control GaugeTarget
        {
            get { return gauge_target; }
            set { gauge_target = value; }
        }

        private Single m_MinValue = 0f;
        public Single MinValue
        {
            get
            {
                return m_MinValue;
            }
            set
            {
                if ((m_MinValue != value)
                && (value < m_MaxValue))
                {
                    m_MinValue = value;
                }
            }
        }

        private Single m_MaxValue = 1000.0f;
        public Single MaxValue
        {
            get
            {
                return m_MaxValue;
            }

        }

        public Point Center
        {
            get
            {
                return m_Center;
            }
            set
            {
                if (m_Center != value)
                {
                    m_Center = value;
                }
            }
        }


        public Single Value
        {
            get
            {
                return m_value;
            }
            set
            {

                // lpf.Update(value);
                //if (value)
                //{
                // float minDisplayValue = 17;
                m_value = value;
                // m_value = Math.Min(Math.Max(value, m_MinValue), m_MaxValue);
                //}
            }
        }

        public int BaseArcStart
        {
            get { return baseArcStart; }
            set { this.baseArcStart = value; }
        }

        public int BaseArcSweep
        {
            get { return baseArcSweep; }

            set
            {
                baseArcSweep = value;
            }
        }


        #endregion

        #region constructor


        int m_id;
        public SMeter(Thetis.Console c, int id, Control target)
        {
            m_id = id;
            this.GaugeTarget = target;
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = (float)(8.25 / ratio);
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            gaugeBitmap = new Bitmap(Thetis.Properties.Resources.OLDAnalogSignalGauge);
            this.PerformLayout();
            float sq = (float)Math.Sqrt(2);
            //lpf = new FilterButterworth(10, 50,FilterButterworth.PassType.Lowpass, sq);
        }

        ~SMeter()
        {

        }

        #endregion

        #region functions

        #region DirectX

#if DirectX

        public bool DirectX_Init(string background_image)
        {
            if (!booting && !DX_reinit)
            {
                try
                {
                    DX_reinit = true;
                    PresentParameters presentParms = new PresentParameters();
                    presentParms.Windowed = true;
                    presentParms.SwapEffect = SwapEffect.Discard;
                    presentParms.Multisample = MultisampleType.None;
                    presentParms.EnableAutoDepthStencil = true;
                    presentParms.AutoDepthStencilFormat = Format.D24X8;
                    presentParms.PresentFlags = PresentFlags.DiscardDepthStencil;
                    presentParms.PresentationInterval = PresentInterval.Default;
                    presentParms.BackBufferFormat = Format.X8R8G8B8;
                    presentParms.BackBufferHeight = gauge_target.Height;
                    presentParms.BackBufferWidth = gauge_target.Width;
                    presentParms.Windowed = true;
                    presentParms.BackBufferCount = 1;

                    switch (directx_render_type)
                    {
                        case RenderType.HARDWARE:
                            {
                                try
                                {
                                    device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        gauge_target.Handle, CreateFlags.HardwareVertexProcessing |
                                        CreateFlags.FpuPreserve | CreateFlags.Multithreaded,
                                        presentParms);
                                }
                                catch (Direct3D9Exception ex)
                                {
                                    if (debug && !console.ConsoleClosing)
                                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                            "DirectX hardware init error(AGauge)!\n" + ex.ToString());
                                }
                            }
                            break;

                        case RenderType.SOFTWARE:
                            {

                                try
                                {
                                    device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        gauge_target.Handle, CreateFlags.SoftwareVertexProcessing |
                                        CreateFlags.FpuPreserve | CreateFlags.Multithreaded, presentParms);
                                }
                                catch (Direct3D9Exception exe)
                                {
                                    if (debug && !console.ConsoleClosing)
                                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                            "DirectX software init error(AGauge)!\n" + exe.ToString());

                                    return false;
                                }
                            }
                            break;
                    }

                    var vertexElems = new[] {
                        new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                        new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                        VertexElement.VertexDeclarationEnd
                        };

                    var vertexDecl = new VertexDeclaration(device, vertexElems);
                    device.VertexDeclaration = vertexDecl;

                    if (background_image != null && File.Exists(background_image))
                    {
                        BackgroundTexture = Texture.FromFile(device, background_image, gauge_target.Width, gauge_target.Height,
                            1, Usage.None, Format.Unknown, Pool.Managed, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);
                    }

                    texture_size.Width = gauge_target.Width;
                    texture_size.Height = gauge_target.Height;
                    sprite = new Sprite(device);

                    verts1 = new Vector2[2];
                    line = new Line(device);
                    line.Antialias = true;
                    line.Width = 3;
                    line.GLLines = true;
                    device.SetRenderState(RenderState.AntialiasedLineEnable, true);
                    DX_reinit = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                    DX_reinit = false;

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                            "Init SMeter error!\n" + ex.ToString());

                    return false;
                }
            }

            return true;
        }

        public void DirectXRelease()
        {
            try
            {
                if (!booting && !DX_reinit)
                {
                    DX_reinit = true;

                    if (device != null)
                    {
                        device.Dispose();
                        device = null;
                    }

                    DX_reinit = false;
                }
            }
            catch (Exception ex)
            {
                Debug.Write("DX release error!" + ex.ToString());
                DX_reinit = false;
            }
        }

        public bool RenderGauge()
        {
            try
            {
                if (device != null && !DX_reinit)
                {
                    Single brushAngle = (Int32)(m_BaseArcStart + (m_value - m_MinValue) * m_BaseArcSweep /
                        (m_MaxValue - m_MinValue)) % 360;
                    Double needleAngle = brushAngle * Math.PI / 180;
         Center = new Point(0, 0);
                    verts1[0].X = (float)(Center.X + m_NeedleRadius / 4 * Math.Cos(needleAngle));
                    verts1[0].Y = (float)(Center.Y + m_NeedleRadius / 4 * Math.Sin(needleAngle));
                    verts1[1].X = (float)(Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                    verts1[1].Y = (float)(Center.Y + m_NeedleRadius * Math.Sin(needleAngle));

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 0.0f, 0);
                    sprite.Begin(SpriteFlags.AlphaBlend);

                    if (BackgroundTexture != null)
                        sprite.Draw(BackgroundTexture, texture_size, (Color4)Color.White);

                    sprite.End();
                    //Begin the scene
                    device.BeginScene();
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.SourceAlpha);
                    device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.DestinationAlpha);
                    line.Draw(verts1, Color.Red);
                    device.EndScene();
                    device.Present();
                    return true;
                }

                if (DX_reinit)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (DX_reinit)
                    return true;
                else
                    return false;
            }
        }

#endif

        #endregion
        private const bool AutoSizeDisplay = true;

        private double m_NeedleWidth = 2;
        double NeedleWidth() { return m_NeedleWidth; }

        #region GDI+

        PointF[] points = new PointF[3];

        public void PaintGauge(PaintEventArgs pe)
        {
            int idx = m_id;
            #region AutoSize
            Single centerFactor = 1;
            var center = Center;


            if (AutoSizeDisplay)
            {
                // NOTE: original size in GSDR was 240*133
                double widthFactor = ((1.0 / (double)(2 * Center.X)) * (double)Size.Width);
                double heightFactor = 1; // ((1.0 / (double)(2 * Center.Y)) * (double)Size.Height);
                centerFactor = (float)Math.Min(widthFactor, heightFactor);
                //center = new Point((int)(center.X * widthFactor), (int)(center.Y * heightFactor));
                // ^^ that code puts the needle centre right AT the centre of the display vertically;
                // We don't want that: we imagine the S-Meter movement is just "out of shot": so,
                // down the bottom somewhere:
                center = new Point((int)(center.X * widthFactor), (int)(Height)); // origin right at the bottom, always, but horizontally centred.

            }
            #endregion
            try
            {
                if (display_engine == DisplayEngine.GDI_PLUS || !Common.console.chkPower.Checked)
                {

                    Graphics g = pe.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(gaugeBitmap, 0, 0, pe.ClipRectangle.Width, pe.ClipRectangle.Height);
                    
                    Single brushAngle = (Int32)(BaseArcStart + (m_value - m_MinValue) * BaseArcSweep /
                        (m_MaxValue - m_MinValue)) % 360;
                    Double needleAngle = brushAngle * Math.PI / 180;

                    points[0].X = (float)(Center.X + m_NeedleRadius / 4 * Math.Cos(needleAngle));
                    points[0].Y = (float)(Center.Y + m_NeedleRadius / 4 * Math.Sin(needleAngle));

                    points[1].X = (float)(Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                    points[1].Y = (float)(Center.Y + m_NeedleRadius * Math.Sin(needleAngle));

                    pe.Graphics.DrawLine(new Pen(Color.Red, 2f), Center.X, Center.Y, points[0].X, points[0].Y);
                    pe.Graphics.DrawLine(new Pen(Color.Red, 2f), Center.X, Center.Y, points[1].X, points[1].Y);
                    
                    /*/
                    Single brushAngle = (Int32)(BaseArcStart + (m_value - m_MinValue) * BaseArcSweep / (m_MaxValue - m_MinValue)) % 360;
                    if (brushAngle < 0) brushAngle += 360;
                    Double needleAngle = brushAngle * Math.PI / 180;

                    int needleWidth = (int)(m_NeedleWidth * centerFactor);
                    int needleRadius = (int)(m_NeedleRadius * centerFactor);

                    Point startPoint = new Point((Int32)(center.X - needleRadius / 8 * Math.Cos(needleAngle)),
                                                (Int32)(center.Y - needleRadius / 8 * Math.Sin(needleAngle)));
                    Point endPoint = new Point((Int32)(center.X + needleRadius * Math.Cos(needleAngle)),
                                             (Int32)(center.Y + needleRadius * Math.Sin(needleAngle)));

                    // fixme: no NEWS in ere!
                    using (var pnLine = new Pen(Color.Red, needleWidth))
                    {
                        g.DrawLine(pnLine, center.X, center.Y, endPoint.X, endPoint.Y);
                        g.DrawLine(pnLine, center.X, center.Y, startPoint.X, startPoint.Y);
                    }
                    /*/
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #endregion
    }
}
