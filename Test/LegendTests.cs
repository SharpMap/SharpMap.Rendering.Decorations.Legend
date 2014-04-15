using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile.Web;
using NetTopologySuite;
using NUnit.Framework;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration.Legend.Factories;

namespace SharpMap.Rendering.Decoration.Legend
{
    public class LegendTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            GeoAPI.GeometryServiceProvider.Instance = NtsGeometryServices.Instance;
        }

        [Test]
        public void Test1()
        {
            using (var map = new Map())
            {
                map.Layers.Add(
                    new TileLayer(
                        new BingTileSource(new BingRequest(BingRequest.UrlBingStaging, string.Empty, BingMapType.Aerial)),
                        "Bing"));
                map.ZoomToExtents();
                var legend = new LegendFactory().Create(map);
                using (var img = legend.GetLegendImage(96))
                {
                    img.Save("legendimage.png");
                    System.Diagnostics.Process.Start("legendimage.png");
                }
            }

        }
    }
}
