using System.Drawing;
using System.Runtime.InteropServices;
using SharpMap.Rendering.Decoration.Legend.Factories;

namespace SharpMap.Rendering.Decoration.Legend
{
    /// <summary>
    /// A legend implementation
    /// </summary>
    public class Legend : MapDecoration, ILegend
    {
        private readonly Map _map;
        private readonly ILegendFactory _factory;

        public Legend(Map map)
        {
            _map = map;
            _factory = new LegendFactory();
            Root = new LegendItem { Expanded = true };
        }
        
        protected override Size InternalSize(Graphics g, Map map)
        {
            return Root.InternalSize(g, map);
        }

        public ILegendFactory Factory { get { return _factory; } }

        /// <summary>
        /// Gets or sets the root item of the legend
        /// </summary>
        public ILegendItem Root { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the indentation performed on
        /// </summary>
        public int Indentation { get; set; }

        /// <summary>
        /// Function to render the actual map decoration
        /// </summary>
        /// <param name="g"/><param name="map"/>
        protected override void OnRender(Graphics g, Map map)
        {
            base.OnRender(g, map);
            Root.Render(g, map);
        }

        /// <summary>
        /// Method to get an image of the decoration
        /// </summary>
        /// <param name="dpi">The dpi setting</param>
        /// <returns>An image of the decoration</returns>
        // TODO: MOVE TO MAPDECORATION, OR AS EXTENSIONMETHOD FOR IMAPDECORATION?
        public Image GetLegendImage(int dpi)
        {
            Size requiredSize;
            using (var bmp = new Bitmap(1, 1))
            {
                bmp.SetResolution(dpi, dpi);
                using (var g = Graphics.FromImage(bmp))
                {
                    requiredSize = InternalSize(g, _map);
                }
            }
            if (requiredSize.Width <= 0 || requiredSize.Height <= 0)
                return null;

            var res = new Bitmap(requiredSize.Width, requiredSize.Height);
            res.SetResolution(dpi,dpi);

            var anchor = Anchor;
            Anchor = MapDecorationAnchor.LeftTop;
            var location = Location;
            Location = Point.Empty;

            using (var g = Graphics.FromImage(res))
            {
                g.Clear(Color.White);
                Render(g, _map);
            }

            Location = location;
            Anchor = anchor;

            return res;
        }

    }
}