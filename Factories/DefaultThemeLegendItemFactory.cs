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
	public class DefaultThemeLegendItemFactory : ILegendItemFactory
	{
		public Type ForType { get { return typeof(ITheme);}}
		
		public ILegendItem Create(ILegend legend, object item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (!(item is ITheme))
				throw new ArgumentException("Item is not a theme", "item");
			
			if (item is GradientTheme)
			{
				return CreateGradientThemeLegendItems(legend, (GradientTheme)item);
			}
			else if (item is UniqueValuesTheme<string>)
			{
				return CreateUniqueValuesLegendItems<string>(legend, (UniqueValuesTheme<string>)item);
			}
			else if (item is UniqueValuesTheme<double>)
			{
				return CreateUniqueValuesLegendItems<double>(legend, (UniqueValuesTheme<double>)item);
			}
			else if (item is UniqueValuesTheme<int>)
			{
				return CreateUniqueValuesLegendItems<int>(legend, (UniqueValuesTheme<int>)item);
			}
			else if (item is CategoryTheme<string>)
			{
				return CreateCategoryThemeLegendItem<string>(legend, (CategoryTheme<string>)item);
			}
			else if (item is CategoryTheme<double>)
			{
				return CreateCategoryThemeLegendItem<double>(legend, (CategoryTheme<double>)item);
			}
			else if (item is CategoryTheme<int>)
			{
				return CreateCategoryThemeLegendItem<int>(legend, (CategoryTheme<int>)item);
			}
			return null;
		}
		
		private static SharpMap.Data.FeatureDataTable CreateTable(string columnName, Type type)
		{
			var fdt = new SharpMap.Data.FeatureDataTable();
			fdt.Columns.Add(new System.Data.DataColumn(columnName, type));
			return fdt;
		}
		
		ILegendItem CreateUniqueValuesLegendItems<T>(ILegend legend, UniqueValuesTheme<T> uvt)
			where T: IConvertible
		{
			var res = new LegendItem{ Label = " ("+ uvt.AttributeName+")",
				LabelFont = legend.Factory.ItemFont, LabelBrush = legend.Factory.ForeColor,
				Indentation = legend.Factory.SymbolSize.Width
			};
			
			var defaultItem = legend.Factory[uvt.DefaultStyle].Create(legend, uvt.DefaultStyle);
			defaultItem.Label = "(default)";
			defaultItem.LabelBrush = legend.Factory.ForeColor;
			defaultItem.LabelFont = legend.Factory.ItemFont;
			res.SubItems.Add(defaultItem);
			using(var fdt = CreateTable(uvt.AttributeName, typeof(T)))
			{
				for (var i = 0; i < uvt.UniqueValues.Length; i++)
				{
					var row = fdt.NewRow();
					T val = default(T);
					row[0] = Convert.ChangeType(uvt.UniqueValues[i], val.GetTypeCode());
					var style = uvt.GetStyle(row);
					var item = legend.Factory[style].Create(legend, style);
					item.Label = uvt.UniqueValues[i];
					item.LabelFont = legend.Factory.ItemFont;
					item.LabelBrush = legend.Factory.ForeColor;
					res.SubItems.Add(item);
				}
			}
			
			return res;
		}

		ILegendItem CreateGradientThemeLegendItems(ILegend legend, GradientTheme gt)
		{
			var res = new LegendItem{ Label = " ("+gt.ColumnName+")" ,
				Indentation = legend.Factory.SymbolSize.Width
			};
			var fcb = gt.FillColorBlend;
			var lcb = gt.LineColorBlend;
			var tcb = gt.TextColorBlend;
			for (var i = 0; i < fcb.Positions.Length; i++)
			{
				var tmp = gt.Min + fcb.Positions[i] * (gt.Max - gt.Min);
				var sl = new LegendItem {
					Label = tmp.ToString("N"),
					LabelFont = legend.Factory.ItemFont,
					LabelBrush = tcb != null ? new SolidBrush(tcb.Colors[i]) : legend.Factory.ForeColor,
					Symbol = CreateSymbol(legend.Factory.SymbolSize, fcb.Colors[i], lcb != null ? lcb.Colors[i] : Color.Transparent),
					
				};
				res.SubItems.Add(sl);
			}
			return res;
			
		}
		
		private static Image CreateSymbol(Size symbolSize, Color fillColor, Color lineColor, int lineWidth = 1)
		{
			var res = new Bitmap(symbolSize.Width, symbolSize.Height);
			using (var g = Graphics.FromImage(res))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				var rect = new Rectangle(2, 2, symbolSize.Width-4, symbolSize.Height - 4);
				using (var b = new SolidBrush(fillColor))
					g.FillRectangle(b, rect);
				using (var p = new Pen(new SolidBrush(lineColor), lineWidth))
					g.DrawRectangle(p, rect);
			}
			return res;
		}
		
		private static ILegendItem CreateCategoryThemeLegendItem<T>(ILegend legend, CategoryTheme<T> ct)
			where T: IComparable<T>
		{
			var res = new LegendItem{ Label = " ("+ ct.ColumnName +")",
				LabelFont = legend.Factory.ItemFont, LabelBrush = legend.Factory.ForeColor,
				Indentation = legend.Factory.SymbolSize.Width
			};
			
			var defaultItem = legend.Factory[ct.Default.Style].Create(legend, ct.Default.Style);
			defaultItem.Label = "(" +  ct.Default.Title + ")";
			defaultItem.LabelBrush = legend.Factory.ForeColor;
			defaultItem.LabelFont = legend.Factory.ItemFont;
			
			res.SubItems.Add(defaultItem);
			foreach (var element in ct.ItemsAsReadOnly())
			{
				var style = element.Style;
				var item = legend.Factory[style].Create(legend, style);
				item.Label = element.Title;
				item.LabelFont = legend.Factory.ItemFont;
				item.LabelBrush = legend.Factory.ForeColor;
				res.SubItems.Add(item);
			}
			
			return res;
		}
	}
}
