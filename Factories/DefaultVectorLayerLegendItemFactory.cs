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
using GeoAPI.Geometries;
using SharpMap.Layers;
using SharpMap.Layers.Symbolizer;
using SharpMap.Rendering.Symbolizer;
using SharpMap.Styles;
using SharpMap.Rendering.Thematics;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
	/// <summary>
	/// Description of DefaultVectorLayerLegendItemFactory.
	/// </summary>
	public class DefaultVectorLayerLegendItemFactory : ILegendItemFactory
	{
		public Type[] ForType 
		{
			get 
			{
				return new [] {typeof(VectorLayer)};
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
		    res.Item = item;
			res.LabelFont = legend.Settings.ItemFont;
            res.LabelBrush = legend.Settings.ForeColor;
            res.Padding = legend.Settings.Padding;
			res.Exclude = !vl.Enabled;
			res.Expanded = res.SubItems.Count > 0;

			return res;
		}
	}

    /// <summary>
    /// 
    /// </summary>
	public class DefaultLabelLayerLegendItemFactory : ILegendItemFactory
    {
	    public Type[] ForType 
        {
	        get { return new [] {typeof (LabelLayer)}; }
	    }

	    public ILegendItem Create(ILegend legend, object item)
	    {
            if (item == null)
                throw new ArgumentNullException("item");
            if (!(item is LabelLayer))
                throw new ArgumentException("Item is not a label layer", "item");

            var vl = (LabelLayer)item;

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

            res.LabelFont = legend.Settings.ItemFont;
            res.LabelBrush = legend.Settings.ForeColor;
            res.Padding = legend.Settings.Padding;
            res.Exclude = !vl.Enabled;
            res.Expanded = res.SubItems.Count > 0;
	        res.Item = item;

            return res;
        }
    }

    public class DefaultSymbolizerLayerLegendItemFactory : ILegendItemFactory
    {
        public Type[] ForType
        {
            get
            {
                return new [] {
                    typeof(AnyGeometryVectorLayer),
                    typeof(BaseVectorLayer<ILineal>),
                    typeof(BaseVectorLayer<IPolygonal>),
                    typeof(BaseVectorLayer<IPuntal>)
                };
            }
        }

        public ILegendItem Create(ILegend legend, object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            if (!(item is AnyGeometryVectorLayer ||
                  item is PuntalVectorLayer || item is LinealVectorLayer || item is PolygonalVectorLayer))
                throw new ArgumentException("Item is not a vector layer", "item");

            var sym = GetSymbolizer(item);

            ILegendItem res = null;
            var symFac = legend.Factory[sym];
            if (symFac != null)
                res = symFac.Create(legend, sym) ?? new LegendItem();
            else
                res = new LegendItem();

            res.Item = item;
            res.Label = ((ILayer) item).LayerName;

            res.LabelFont = legend.Settings.ItemFont;
            res.LabelBrush = legend.Settings.ForeColor;
            res.Padding = legend.Settings.Padding;
            res.Exclude = !((ILayer)item).Enabled;
            res.Expanded = res.SubItems.Count > 0;

            return res;
        }

        private ISymbolizer GetSymbolizer(object item)
        {
            var agl = item as AnyGeometryVectorLayer;
            if (agl != null)
            {
                return agl.Symbolizer;
            }

            if (item is BaseVectorLayer<IPuntal>)
                return ((BaseVectorLayer<IPuntal>) item).Symbolizer;

            if (item is BaseVectorLayer<ILineal>)
                return ((BaseVectorLayer<ILineal>)item).Symbolizer;

            if (item is BaseVectorLayer<IPolygonal>)
                return ((BaseVectorLayer<IPolygonal>)item).Symbolizer;

            throw new Exception("Should never reach here!");
        }
    }
}
