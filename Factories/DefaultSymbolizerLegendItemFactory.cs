using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using GeoAPI.Geometries;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Layers.Symbolizer;
using SharpMap.Rendering.Symbolizer;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    public class DefaultSymbolizerLegendItemFactory : ILegendItemFactory
    {
        public Type[] ForType { get { return new [] {typeof (ISymbolizer)}; } }

        private static bool IsPointSymbolizer(ISymbolizer symolizer)
        {
            if (symolizer is ISymbolizer<IPuntal>)
                return true;
            if (symolizer is ISymbolizer<IGeometry>)
                return true;
            return false;
        }

        private static bool IsLineSymbolizer(ISymbolizer symolizer)
        {
            if (symolizer is ISymbolizer<ILineal>)
                return true;
            if (symolizer is ISymbolizer<IGeometry>)
                return true;
            return false;
        }

        private static bool IsPolygonSymbolizer(ISymbolizer symolizer)
        {
            if (symolizer is ISymbolizer<IPolygonal>)
                return true;
            if (symolizer is ISymbolizer<IGeometry>)
                return true;
            return false;
        }

        private static Map CreateMap(Size size, ISymbolizer symbolizer)
        {
            var map = new Map(size);
            map.ZoomToBox(new Envelope(0, size.Width, 0, size.Height));
            return map;
        }

        private static IGeometry CreatePoint(IGeometryFactory factory, Size size)
        {
            return factory.CreatePoint(new Coordinate(size.Width/2d, size.Height/2d));
        }

        private static IGeometry CreateLine(IGeometryFactory factory, Size size)
        {
            return factory.CreateLineString( new []
            {
                new Coordinate(2, 2),
                new Coordinate(0.3f * size.Width, 0.5*size.Height),
                new Coordinate(0.65f * size.Width, 0.5*size.Height+1),
                new Coordinate(size.Width -4, size.Height -4),
            });
        }

        private static IGeometry CreatePolygon(IGeometryFactory factory, Size size)
        {
            return factory.ToGeometry(new Envelope(2, size.Width - 4, 2, size.Height - 4));
        }

        public ILegendItem Create(ILegend legend, object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (!(item is ISymbolizer))
                throw new ArgumentException("item is not a ISymbolizer class");

            var sym = (ISymbolizer) item;

            var res = new LegendItem();
            if (IsPointSymbolizer(sym))
                res.SubItems.Add(CreateSymbolizerLegendItem(legend.Factory, CreatePuntalSymbol(legend.Factory.SymbolSize, sym)));
            if (IsLineSymbolizer(sym))
                res.SubItems.Add(CreateSymbolizerLegendItem(legend.Factory, CreateLinealSymbol(legend.Factory.SymbolSize, sym)));
            if (IsPolygonSymbolizer(sym))
                res.SubItems.Add(CreateSymbolizerLegendItem(legend.Factory, CreatePolygonalSymbol(legend.Factory.SymbolSize, sym)));

            if (res.SubItems.Count == 0)
                return new LegendItem();
            if (res.SubItems.Count == 1)
                return res.SubItems.First();

            res.Expanded = true;
            res.Padding = legend.Factory.Padding;
            res.Indentation = legend.Factory.SymbolSize.Width;

            return res;
        }

        private static ILegendItem CreateSymbolizerLegendItem(ILegendFactory factory, Image symbol)
        {
            return new LegendItem
            {
                Symbol =  symbol,
                LabelFont = factory.ItemFont
            };

        }

        internal static Image CreatePuntalSymbol(Size symbolSize, ISymbolizer sym)
        {
            using (var map = new Map(symbolSize))
            {
                var l = new PuntalVectorLayer("0", new GeometryProvider(
                    map.Factory.CreatePoint(new Coordinate(symbolSize.Width / 2d, symbolSize.Height / 2d))));
                var s = (IPointSymbolizer)sym.Clone();

                s.Offset = new PointF(0, 0);
                
                if (s.Size.Width > symbolSize.Width)
                    s.Scale = (float)symbolSize.Width / s.Size.Width;
                if (s.Size.Height * s.Scale > symbolSize.Height)
                    s.Scale = (float)symbolSize.Height / s.Size.Height;
                s.Scale *= 0.8f;

                l.Symbolizer = s;

                map.Layers.Add(l);
                map.ZoomToBox(new Envelope(0, symbolSize.Width, 0, symbolSize.Height));

                return map.GetMap();
            }
        }

        internal static Image CreateLinealSymbol(Size symbolSize, ISymbolizer sym)
        {
            using (var map = new Map(symbolSize))
            {
                var l = new LinealVectorLayer("0", new GeometryProvider(
                    map.Factory.CreateLineString(new[] { new Coordinate(2, 2), new Coordinate(8, 12), new Coordinate(12, 11), new Coordinate(22, 2) })));
                l.Symbolizer = (ILineSymbolizer)sym.Clone();

                map.Layers.Add(l);
                map.ZoomToBox(new Envelope(0, symbolSize.Width, 0, symbolSize.Height));

                return map.GetMap();
            }
        }

        public static Image CreatePolygonalSymbol(Size symbolSize, ISymbolizer sym)
        {
            using (var map = new Map(symbolSize))
            {
                var l = new PolygonalVectorLayer("0", new GeometryProvider(
                    map.Factory.ToGeometry(new Envelope(2, symbolSize.Width - 4, 2, symbolSize.Height - 4))));
                l.Symbolizer = (IPolygonSymbolizer)sym.Clone();

                map.Layers.Add(l);
                map.ZoomToBox(new Envelope(0, symbolSize.Width, 0, symbolSize.Height));

                return map.GetMap();
            }
        }
    }
}