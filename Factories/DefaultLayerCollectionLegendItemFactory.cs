using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration.Legend.Properties;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    public class DefaultLayerCollectionLegendItemFactory : AbstractLegendItemFactory
    {
        public override Type[] ForType
        {
            get { return new[] { typeof(LayerCollection) }; }
        }

        public override ILegendItem Create(ILegend legend, object item)
        {
            var res = base.Create(legend, item);

            if (item == legend.Map.VariableLayers)
            {
                res.Label = Resources.lcVariable;
            }
            else if (item == legend.Map.Layers)
            {
                res.Label = Resources.lcStatic;
            }
            else if (item == legend.Map.BackgroundLayer)
            {
                res.Label = Resources.lcBackground;
            }
            else
            {
                throw new ArgumentException("The layer collection does not belong to the map for this legend");
            }
            return res;
        }

        protected override string CreateLabel(object item)
        {
            return string.Empty;
        }

        protected override Image CreateSymbol(Size symbolSize, object item)
        {
            var res = new Bitmap(symbolSize.Width, symbolSize.Height);
            using (var g = Graphics.FromImage(res))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(Resources.lcSymbol, new Rectangle(0, 0, symbolSize.Width, symbolSize.Height), 
                                                new Rectangle(0, 0, Resources.lcSymbol.Width, Resources.lcSymbol.Height),
                                                GraphicsUnit.Pixel);
            }
            return res;
        }
    }
}