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
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using SharpMap.Layers;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    internal class DefaultLayerGroupLegendItemFactory : AbstractLegendItemFactory 
    {
        public override Type[] ForType { get { return new [] {typeof (LayerGroup)}; }}

        public override ILegendItem Create(ILegendSettings settings, object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!(item is LayerGroup))
                throw new ArgumentException("Not of valid type", "item");

            var layerGroup = (LayerGroup) item;

            var res = new LayerGroupLegendItem
            {
                Label = layerGroup.LayerName,
                LabelFont = settings.ItemFont,
                LabelBrush = settings.ForeColor,
                Item = item
            };

            var lrs = new Layer[layerGroup.Layers.Count];
            layerGroup.Layers.CopyTo(lrs, 0);
            Array.Reverse(lrs);
            foreach (var layer in lrs)
            {
                var lif = Factory[layer];
                if (lif != null)
                {
                    var nli = lif.Create(settings, layer);
                    nli.Parent = res;
                    res.SubItems.Add(nli);
                }
            }

            return res;
        }

    }

    internal class LayerGroupLegendItem : LegendItem
    {
        private static readonly Image DefaultExpandedSymbol;
        private static readonly Image DefaultCollapsedSymbol;

        static LayerGroupLegendItem()
        {
            var a = Assembly.GetExecutingAssembly();
            using (var ds = a.GetManifestResourceStream("SharpMap.Rendering.Decoration.Legend.Factories.Default.png"))
            {
                if (ds != null)
                {
                    using (var def = Image.FromStream(ds))
                    {
                        DefaultCollapsedSymbol = new Bitmap(24, 24, PixelFormat.Format24bppRgb);
                        using (var g = Graphics.FromImage(DefaultCollapsedSymbol))
                            g.DrawImage(def, new Rectangle(0, 0, 24, 24), new Rectangle(0, 0, 24, 24),
                                GraphicsUnit.Pixel);

                        DefaultExpandedSymbol = new Bitmap(24, 24, PixelFormat.Format24bppRgb);
                        using (var g = Graphics.FromImage(DefaultExpandedSymbol))
                            g.DrawImage(def, new Rectangle(0, 0, 24, 24), new Rectangle(24, 0, 24, 24),
                                GraphicsUnit.Pixel);
                    }
                }
            }
        }

        private Image _expandedSymbol;
        private Image _collapsedSymbol;

        public Image ExpandedSymbol
        {
            get { return _expandedSymbol ?? DefaultExpandedSymbol; }
            set { _expandedSymbol = value; }
        }

        public Image CollapsedSymbol
        {
            get { return _collapsedSymbol ?? DefaultCollapsedSymbol; }
            set { _collapsedSymbol = value; }
        }

        public override bool Expanded
        {
            get { return SubItems.Count > 0 && base.Expanded; }
            set { base.Expanded = value; }
        }

        public override Image Symbol
        {
            get { return Expanded ? ExpandedSymbol : CollapsedSymbol; }
        }
    }
}