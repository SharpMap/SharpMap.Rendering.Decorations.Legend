// Copyright 2014 - Felix Obermaier (www.ivv-aachen.de)
//
// This file is part of SharpMap.Rendering.Decoration.Legend.
// SharpMap.Rendering.Decoration.Legend is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap.Rendering.Decoration.Legend is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Drawing;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.Rendering.Thematics;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
	public class DefaultVectorStyleLegendItemFactory : ILegendItemFactory
	{
		private static bool HasPuntalStyle(VectorStyle style)
		{
			return ((style.Symbol != null) ||
			        (style.PointColor != null && style.PointColor != System.Drawing.Brushes.Transparent)
			       );
		}
		
		private static bool HasLinealStyle(VectorStyle style)
		{
			if (style.Line == null)
				return false;
			
			var b = style.Line.Brush as SolidBrush;
			if (b != null && b.Color.ToArgb() == Color.Transparent.ToArgb())
				return false;
			
			return true;
		}
		
		private static bool HasPolygonalStyle(VectorStyle style)
		{
			if (style.Fill == null)
				return false;
			
			var b = style.Fill as SolidBrush;
			if (b != null && b.Color.ToArgb() == Color.Transparent.ToArgb())
				return false;
			
			return true;
		}
		
		public Type ForType { get { return typeof(VectorStyle);}}
		
		public ILegendItem Create(ILegend legend, object item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (!(item is VectorStyle))
			    throw new ArgumentException("item is not a VectorStyle", "item");
			
			var vs = (VectorStyle)item;
			
			var lis = new List<ILegendItem>(3);
			if (HasPuntalStyle(vs)) 
				lis.Add(CreatePuntalStyleLegendItem(legend.Factory, vs));
			if (HasLinealStyle(vs)) 
				lis.Add(CreateLinealStyleLegendItem(legend.Factory, vs));
			if (HasPolygonalStyle(vs)) 
				lis.Add(CreatePolygonalStyleLegendItem(legend.Factory, vs));
			
			if (lis.Count < 1)
				return new LegendItem();
			if (lis.Count == 1)
				return lis[0];
	
			var res = new LegendItem();
			res.Indentation = legend.Factory.SymbolSize.Width;
				foreach (var li in lis) {
					res.SubItems.Add(li);
				}
				return res;
		}
		
		protected virtual ILegendItem CreatePuntalStyleLegendItem(ILegendFactory factory, VectorStyle vs)
		{
			return new LegendItem { Symbol = CreatePointSymbol(factory.SymbolSize, vs) };
		}
		
		protected virtual ILegendItem CreateLinealStyleLegendItem(ILegendFactory factory, VectorStyle vs)
		{
			return new LegendItem { Symbol = CreateLineSymbol(factory.SymbolSize, vs) };
		}
		
		protected virtual ILegendItem CreatePolygonalStyleLegendItem(ILegendFactory factory, VectorStyle vs)
		{
			return new LegendItem { Symbol = CreatePolygonalSymbol(factory.SymbolSize, vs) };
		}
		
		System.Drawing.Image CreatePointSymbol(Size symbolSize, VectorStyle vs)
		{
			var res = new System.Drawing.Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (vs.Symbol != null)
				{
					g.DrawImage(vs.Symbol, new Rectangle(0, 0, symbolSize.Width, symbolSize.Height),
							               new Rectangle(0, 0, vs.Symbol.Width, vs.Symbol.Height), GraphicsUnit.Pixel);
				}
				else if (vs.PointColor != null && vs.PointColor != Brushes.Transparent)
				{
					g.FillEllipse(vs.PointColor, new RectangleF(0, 0, symbolSize.Width, symbolSize.Height));
				}
				else
				{
					
				}
			}
			return res;
		}
		
		System.Drawing.Image CreateLineSymbol(Size symbolSize, VectorStyle vs)
		{
			var res = new System.Drawing.Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (vs.LineSymbolizer == null)
				{
					var pts = new [] { new Point(2,22), new Point(8, 12), new Point(12, 13), new Point(22, 2)};
					if (vs.EnableOutline)
						g.DrawLines(vs.Outline, pts);
					g.DrawLines(vs.Line, pts);
				}
			}
			return res;
		}
	
		System.Drawing.Image CreatePolygonalSymbol(Size symbolSize, VectorStyle vs)
		{
			var res = new System.Drawing.Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (vs.PolygonSymbolizer == null)
				{
					var rect = new Rectangle(3,3, 18, 18);
					g.FillRectangle(vs.Fill, rect);
					g.DrawRectangle(vs.Outline, rect);
				}
			}
			return res;
		}
	
	}
}
