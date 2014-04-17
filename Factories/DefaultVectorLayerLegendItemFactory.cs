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
	/// <summary>
	/// Description of DefaultVectorLayerLegendItemFactory.
	/// </summary>
	public class DefaultVectorLayerLegendItemFactory : ILegendItemFactory
	{
		public DefaultVectorLayerLegendItemFactory()
		{
		}
		
		public Type ForType 
		{
			get 
			{
				return typeof(VectorLayer);
			}
		}
		
		public ILegendItem Create(ILegend legend, object item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (!(item is VectorLayer))
				throw new ArgumentException("Item is not a vector layer", "item");
			
			var vl = (VectorLayer)item;
			
			ILegendItem res = null;
			if (vl.Theme != null)
			{
				res = legend.Factory[vl.Theme].Create(legend, vl.Theme) ?? new LegendItem();
				res.Label = vl.LayerName + res.Label ?? string.Empty;
			}
			else
			{
				res = legend.Factory[vl.Style].Create(legend, vl.Style) ?? new LegendItem();
				res.Label = vl.LayerName;
			}

			res.LabelFont = legend.Factory.ItemFont;
			res.LabelBrush = legend.Factory.ForeColor;
			res.Padding = legend.Factory.Padding;
			res.Exclude = !vl.Enabled;
			res.Expanded = res.SubItems.Count > 0;

			return res;
		}
		
	}
	


}
