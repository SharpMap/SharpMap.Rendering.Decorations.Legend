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
using System.Runtime.InteropServices;
using GeoAPI.Geometries;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Layers.Symbolizer;
using SharpMap.Rendering.Symbolizer;
using SharpMap.Styles;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
	public class DefaultVectorStyleLegendItemFactory : AbstractLegendItemFactory
	{
	    private static bool HasPuntalStyle(VectorStyle style)
		{
			return ((style.Symbol != null) ||
			        (style.PointColor != null && style.PointColor != Brushes.Transparent)
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

	    public override Type[] ForType
	    {
	        get { return new [] {typeof(VectorStyle)};; }
	    }

		public override ILegendItem Create(ILegendSettings settings, object item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (!(item is VectorStyle))
			    throw new ArgumentException("item is not a VectorStyle", "item");
			
			var vs = (VectorStyle)item;
			
			var lis = new List<ILegendItem>(3);
			if (HasPuntalStyle(vs)) 
				lis.Add(CreatePuntalStyleLegendItem(Factory, settings, vs));
			if (HasLinealStyle(vs)) 
				lis.Add(CreateLinealStyleLegendItem(Factory, settings, vs));
			if (HasPolygonalStyle(vs)) 
				lis.Add(CreatePolygonalStyleLegendItem(Factory, settings, vs));
			
			if (lis.Count < 1)
				return new LegendItem();
			if (lis.Count == 1)
				return lis[0];
	
			var res = new LegendItem();
            res.Indentation = settings.SymbolSize.Width;
		    res.Item = item;
			foreach (var li in lis) 
            {
				{
				    li.Parent = res;
				    li.Item = vs;
				    res.SubItems.Add(li);
				}
			}
			return res;
		}

	    protected virtual ILegendItem CreatePuntalStyleLegendItem(ILegendFactory factory, ILegendSettings settings, VectorStyle vs)
		{
            return new LegendItem { Symbol = CreatePointSymbol(factory, settings.SymbolSize, vs) };
		}

        protected virtual ILegendItem CreateLinealStyleLegendItem(ILegendFactory factory, ILegendSettings settings, VectorStyle vs)
		{
            return new LegendItem { Symbol = CreateLineSymbol(factory, settings.SymbolSize, vs) };
		}

        protected virtual ILegendItem CreatePolygonalStyleLegendItem(ILegendFactory factory, ILegendSettings settings, VectorStyle vs)
		{
            return new LegendItem { Symbol = CreatePolygonSymbol(factory, settings.SymbolSize, vs) };
		}
		
		private static Image CreatePointSymbol(ILegendFactory legend, Size symbolSize, VectorStyle vs)
		{
			var res = new System.Drawing.Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (vs.Symbol != null)
				{
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					g.DrawImage(vs.Symbol, new Rectangle(2, 2, symbolSize.Width-4, symbolSize.Height-4),
							               new Rectangle(0, 0, vs.Symbol.Width, vs.Symbol.Height), GraphicsUnit.Pixel);
				}
				else if (vs.PointColor != null && vs.PointColor != Brushes.Transparent)
				{
					g.FillEllipse(vs.PointColor, new RectangleF(2, 2, symbolSize.Width-4, symbolSize.Height-4));
				}
				else
				{
				    return CreateSymbolizerImage(legend, vs.PointSymbolizer);
				}
			}
			return res;
		}

	    private static Image CreateSymbolizerImage(ILegendFactory factory, ISymbolizer symbolizer)
	    {
            var lif = factory[symbolizer];
            if (lif != null)
            {
                var item = lif.Create(null, symbolizer);
                return item.Symbol;
            }
            return null;
        }
		
		private static Image CreateLineSymbol(ILegendFactory legend, Size symbolSize, VectorStyle vs)
		{
			var res = new Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (vs.LineSymbolizer == null)
				{
                    var pts = new[] { new Point(2, symbolSize.Height - 4), new Point(symbolSize.Width/3, symbolSize.Height / 2), new Point(symbolSize.Width / 2, symbolSize.Height / 2 + 1), new Point(symbolSize.Width -4, 2) };
					if (vs.EnableOutline)
						g.DrawLines(vs.Outline, pts);
					g.DrawLines(vs.Line, pts);
				}
				else
				{
                    return CreateSymbolizerImage(legend, vs.LineSymbolizer);
                }
			}
			return res;
		}
	
		private static Image CreatePolygonSymbol(ILegendFactory factory, Size symbolSize, VectorStyle vs)
		{
			var res = new Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (vs.PolygonSymbolizer == null)
				{
                    var rect = new Rectangle(3, 3, symbolSize.Width - 6, symbolSize.Height - 6);
					g.FillRectangle(vs.Fill, rect);
					g.DrawRectangle(vs.Outline, rect);
				}
                else
				{
                    return CreateSymbolizerImage(factory, vs.PolygonSymbolizer);
				}
            }
			return res;
		}
	
	}
}
