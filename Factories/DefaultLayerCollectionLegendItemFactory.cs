using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
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

        public override ILegendItem Create(ILegendSettings legend, object item)
        {
            var lc = (LayerCollection)item;
            if (lc.Count == 0) 
                return null;

            var res = base.Create(legend, item);

            var lrs = new ILayer[lc.Count];
            lc.CopyTo(lrs, 0);
            Array.Reverse(lrs);
            foreach (var layer in lrs)
            {
                var li = Factory[layer].Create(legend, layer);
                if (li != null)
                {
                    res.SubItems.Add(li);
                }
            }
            res.Expanded = res.SubItems.Count > 0;
            res.Indentation = legend.Indentation;
            return res;
        }

        protected override string CreateLabel(object item)
        {
            var lc = (LayerCollection)item;
            if (item is VariableLayerCollection)
                return Resources.lcVariable;
            if (lc.Count > 0)
            {
                if (lc[0] is TileAsyncLayer)
                    return Resources.lcBackground;
                return Resources.lcStatic;
            }
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