using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using SharpMap.Layers;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    internal class DefaultLayerGroupLegendItemFactory : ILegendItemFactory
    {
        public Type ForType { get { return typeof (LayerGroup); }}

        public ILegendItem Create(ILegend legend, object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!(item is LayerGroup))
                throw new ArgumentException("Not of valid type", "item");

            var layerGroup = (LayerGroup) item;

            var res = new LayerGroupLegendItem 
            {
                Label = layerGroup.LayerName, LabelFont = legend.Factory.ItemFont, LabelBrush = legend.Factory.ForeColor
            };

            foreach (var layer in layerGroup.Layers)
            {
                var lif = legend.Factory[layer];
                if (lif != null)
                {
                    res.SubItems.Add(lif.Create(legend, layer));
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