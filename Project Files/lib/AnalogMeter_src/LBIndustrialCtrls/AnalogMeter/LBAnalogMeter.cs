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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LBSoft.IndustrialCtrls.Utils;

namespace LBSoft.IndustrialCtrls.Meters
{
	/// <summary>
	/// Class for the analog meter control
	/// </summary>
	public partial class LBAnalogMeter : UserControl
	{		
		#region Enumerator
		public enum AnalogMeterStyle
		{
			Circular	= 0,
		};
		#endregion
		
		#region Properties variables
		private	AnalogMeterStyle 		meterStyle;
		private Color					bodyColor;
		private Color					needleColor;
		private Color					scaleColor;
		private bool					viewGlass;
		private double					currValue;
		private double					minValue;
		private double					maxValue;
		private int						scaleDivisions;
		private int						scaleSubDivisions;
		private LBAnalogMeterRenderer	renderer;
		#endregion

		#region Class variables
		protected PointF 				needleCenter;
		protected RectangleF			drawRect;		
		protected RectangleF			glossyRect;		
		protected RectangleF			needleCoverRect;
		protected float					startAngle;
		protected float					endAngle;
		protected float					drawRatio;
		protected LBAnalogMeterRenderer	defaultRenderer;
		#endregion
		
		#region Costructors
		public LBAnalogMeter()
		{
			// Initialization
			InitializeComponent();
			
			// Properties initialization
			this.bodyColor = Color.Red;
			this.needleColor = Color.Yellow;
			this.scaleColor = Color.White;
			this.meterStyle = AnalogMeterStyle.Circular;
			this.viewGlass = false;
			this.startAngle = 225;
			this.endAngle = 315;
			this.minValue = 0;
			this.maxValue = 1;
			this.currValue = 0;
			this.scaleDivisions = 10;
			this.scaleSubDivisions = 10;
			this.renderer = null;
			
			// Set the styles for drawing
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			
			// Create the default renderer
			this.defaultRenderer = new LBDefaultAnalogMeterRenderer();
			this.defaultRenderer.AnalogMeter = this;
		}
		#endregion
		
		#region Properties
		[
			Category("Appearance"),
			Description("Style of the control")
		]
		public AnalogMeterStyle MeterStyle
		{
			get { return meterStyle;}
			set
			{
				meterStyle = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Appearance"),
			Description("Color of the body of the control")
		]
		public Color BodyColor
		{
			get { return bodyColor; }
			set
			{
				bodyColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Appearance"),
			Description("Color of the needle")
		]
		public Color NeedleColor
		{
			get { return needleColor; }
			set
			{
				needleColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Appearance"),
			Description("Show or hide the glass effect")
		]
		public bool ViewGlass
		{
			get { return viewGlass; }
			set
			{
				viewGlass = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Appearance"),
			Description("Color of the scale of the control")
		]
		public Color ScaleColor
		{
			get { return scaleColor; }
			set
			{
				scaleColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Behavior"),
			Description("Value of the data")
		]
		public double Value
		{
			get { return currValue; }
			set
			{
				double val = value;
				if ( val > maxValue )
					val = maxValue;
				
				if ( val < minValue )
					val = minValue;
				
				currValue = val;
				Invalidate();
			}
		}
		
		
		[
			Category("Behavior"),
			Description("Minimum value of the data")
		]
		public double MinValue
		{
			get { return minValue; }
			set
			{
				minValue = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Behavior"),
			Description("Maximum value of the data")
		]
		public double MaxValue
		{
			get { return maxValue; }
			set
			{
				maxValue = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Appearance"),
			Description("Number of the scale divisions")
		]
		public int ScaleDivisions
		{
			get { return scaleDivisions; }
			set
			{
				scaleDivisions = value;
				CalculateDimensions();
				Invalidate();
			}
		}
		
		
		[
			Category("Appearance"),
			Description("Number of the scale subdivisions")
		]
		public int ScaleSubDivisions
		{
			get { return scaleSubDivisions; }
			set
			{
				scaleSubDivisions = value;
				CalculateDimensions();
				Invalidate();
			}
		}

		[Browsable(false)]
		public LBAnalogMeterRenderer Renderer
		{
			get { return this.renderer; }
			set
			{
				this.renderer = value;
				if ( this.renderer != null )
					renderer.AnalogMeter = this;
				Invalidate();
			}
		}
		#endregion
		
		#region Public methods
		public float GetDrawRatio()
		{
			return this.drawRatio;
		}
		
		public float GetStartAngle()
		{
			return this.startAngle;
		}
		
		public float GetEndAngle()
		{
			return this.endAngle;
		}
		
		public PointF GetNeedleCenter()
		{
			return this.needleCenter;
		}
		#endregion
		
		#region Events delegates
		protected override void OnSizeChanged ( EventArgs e )
		{
			base.OnSizeChanged( e );
			
			// Calculate dimensions
			CalculateDimensions();
			
			this.Invalidate();
		}
		
		protected override void OnPaintBackground( PaintEventArgs e )
		{
		}
		
		protected override void OnPaint ( PaintEventArgs e )
		{
			RectangleF _rc = new RectangleF(0, 0, this.Width, this.Height );
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			
			if ( this.renderer == null )
			{
				this.defaultRenderer.DrawBackground( e.Graphics, _rc );
				this.defaultRenderer.DrawBody( e.Graphics, drawRect );
				this.defaultRenderer.DrawThresholds( e.Graphics, drawRect );
				this.defaultRenderer.DrawDivisions( e.Graphics, drawRect );
				this.defaultRenderer.DrawUM( e.Graphics, drawRect );
				this.defaultRenderer.DrawValue( e.Graphics, drawRect );
				this.defaultRenderer.DrawNeedle( e.Graphics, drawRect );
				this.defaultRenderer.DrawNeedleCover( e.Graphics, this.needleCoverRect );
				this.defaultRenderer.DrawGlass( e.Graphics, this.glossyRect );
			}
			else
			{
				if ( this.Renderer.DrawBackground( e.Graphics, _rc ) == false )
				    this.defaultRenderer.DrawBackground( e.Graphics, _rc );
				if ( this.Renderer.DrawBody( e.Graphics, drawRect ) == false )
					this.defaultRenderer.DrawBody( e.Graphics, drawRect );
				if ( this.Renderer.DrawThresholds( e.Graphics, drawRect ) == false )
					this.defaultRenderer.DrawThresholds( e.Graphics, drawRect );
				if ( this.Renderer.DrawDivisions( e.Graphics, drawRect ) == false )
					this.defaultRenderer.DrawDivisions( e.Graphics, drawRect );
				if ( this.Renderer.DrawUM( e.Graphics, drawRect ) == false )
					this.defaultRenderer.DrawUM( e.Graphics, drawRect );
				if ( this.Renderer.DrawValue( e.Graphics, drawRect ) == false )
					this.defaultRenderer.DrawValue( e.Graphics, drawRect );
				if ( this.Renderer.DrawNeedle( e.Graphics, drawRect ) == false )
					this.defaultRenderer.DrawNeedle( e.Graphics, drawRect );
				if ( this.Renderer.DrawNeedleCover( e.Graphics, this.needleCoverRect ) == false )
					this.defaultRenderer.DrawNeedleCover( e.Graphics, this.needleCoverRect );
				if ( this.Renderer.DrawGlass( e.Graphics, this.glossyRect ) == false ) 
					this.defaultRenderer.DrawGlass( e.Graphics, this.glossyRect );				
			}
		}
		#endregion
		
		#region Virtual functions		
		protected virtual void CalculateDimensions()
		{
			// Rectangle
			float x, y, w, h;
			x = 0;
			y = 0;
			w = this.Size.Width;
			h = this.Size.Height;
			
			// Calculate ratio
			drawRatio = (Math.Min(w,h)) / 200;
			if ( drawRatio == 0.0 )
				drawRatio = 1;
		
			// Draw rectangle
			drawRect.X = x;
			drawRect.Y = y;
			drawRect.Width = w - 2;
			drawRect.Height = h - 2;
		
			//if ( w < h )
			//	drawRect.Height = w;
			//else if ( w > h )
			//	drawRect.Width = h;
			
			if ( drawRect.Width < 10 )
				drawRect.Width = 10;
			if ( drawRect.Height < 10 )
				drawRect.Height = 10;
		
			// Calculate needle center
			//needleCenter.X = drawRect.X + ( drawRect.Width / 2 );
			// needleCenter.Y =  drawRect.Y + ( drawRect.Height / 2 );

			needleCenter.X = drawRect.X + (drawRect.Width / 2);
			needleCenter.Y = drawRect.Height + (15 * drawRatio); 

			// Needle cover rect
			var cover = 45;
			needleCoverRect.X = needleCenter.X - (cover/2 * drawRatio );
			needleCoverRect.Y = needleCenter.Y - ( cover/2 * drawRatio );
			needleCoverRect.Width = cover * drawRatio;
			needleCoverRect.Height =  cover * drawRatio;
			
			// Glass effect rect
			glossyRect.X = drawRect.X + ( 20 * drawRatio );
			glossyRect.Y = drawRect.Y + ( 20 * drawRatio );
			glossyRect.Width = drawRect.Width - ( 30 * drawRatio );
			glossyRect.Height = needleCenter.Y + ( 30 * drawRatio );
		}
		#endregion
	}
}
