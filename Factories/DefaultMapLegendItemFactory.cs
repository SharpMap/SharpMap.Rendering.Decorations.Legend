using System;
using System.Drawing;


namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    public class DefaultMapLegendItemFactory : AbstractLegendItemFactory
    {
        public override Type[] ForType
        {
            get { return new[] {typeof (Map)}; }
        }

        public override ILegendItem Create(ILegend legend, object item)
        {
            var res = base.Create(legend, item);

            var map = (Map) item;
            var scale = string.Format("1:{0:N0}", map.MapScale);
            res.SubItems.Add(legend.Factory[scale].Create(legend, scale));
            res.SubItems.Add(legend.Factory[map.BackgroundLayer].Create(legend, map.VariableLayers));
            res.SubItems.Add(legend.Factory[map.BackgroundLayer].Create(legend, map.Layers));
            res.SubItems.Add(legend.Factory[map.BackgroundLayer].Create(legend, map.BackgroundLayer));

            return res;
        }

        protected override string CreateLabel(object item)
        {
            return Properties.Resources.map;
        }
    }

    public class DefaultTextLegendItemFactory : AbstractLegendItemFactory
    {
        public override Type[] ForType
        {
            get { return new[] { typeof(string) }; }
        }

        protected override string CreateLabel(object item)
        {
            return (string) item;
        }
    }

    public class DefaultImageLegendItemFactory : AbstractLegendItemFactory
    {
        public override Type[] ForType
        {
            get { return new[] { typeof(Image) }; }
        }

        protected override string CreateLabel(object item)
        {
            return string.Empty;
        }

        protected override Image CreateSymbol(Size symbolSize, object item)
        {
            return (Image) item;
        }
    }
}