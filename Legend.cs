using System.Drawing;
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
        private ILegendSettings _settings;

        public Legend(Map map, ILegendFactory factory = null)
        {
            _map = map;
            _factory = factory ?? new LegendFactory();
            _settings = _factory.LegendSettings;

            Root = new LegendItem { Expanded = true };
        }


        /// <summary>
        /// Gets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
        }

        /// <summary>
        /// Method to regenerate the map legend
        /// </summary>
        public void Regenerate()
        {
            var root = _factory[_map].Create(this, _map);
            Root = root;
        }

        protected override Size InternalSize(Graphics g, Map map)
        {
            return Root.InternalSize(g, map);
        }

        /// <summary>
        /// Gets the factory that has been used to create this legend
        /// </summary>
        public ILegendFactory Factory { get { return _factory; } }

        /// <summary>
        /// Gets the settings of the legend
        /// </summary>
        public ILegendSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// Gets or sets the root item of the legend
        /// </summary>
        public ILegendItem Root { get; set; }

        private System.Drawing.Drawing2D.SmoothingMode _smoothingMode;
        
		protected override void OnRendering(Graphics g, Map map)
		{
			base.OnRendering(g, map);
            _smoothingMode = g.SmoothingMode;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
		}
        /// <summary>
        /// Function to render the actual map decoration
        /// </summary>
        /// <param name="g"/><param name="map"/>
        protected override void OnRender(Graphics g, Map map)
        {
            base.OnRender(g, map);
            g.TranslateTransform(BorderMargin.Width, BorderMargin.Height);
            Root.Render(g, map);
            g.TranslateTransform(-BorderMargin.Width, - BorderMargin.Height);
        }

		protected override void OnRendered(Graphics g, Map map)
		{
            g.SmoothingMode = _smoothingMode;
			base.OnRendered(g, map);
		}
        /// <summary>
        /// Method to get an image of the decoration
        /// </summary>
        /// <param name="dpi">The dpi setting</param>
        /// <returns>An image of the decoration</returns>
        // TODO: MOVE TO MAPDECORATION, OR AS EXTENSIONMETHOD FOR IMAPDECORATION?
        public Image GetLegendImage(int dpi = 96)
        {
            var anchor = Anchor;
            Anchor = MapDecorationAnchor.LeftTop;
            var location = Location;
            Location = Point.Empty;

            Size requiredSize;
            using (var bmp = new Bitmap(1, 1))
            {
                bmp.SetResolution(dpi, dpi);
                using (var g = Graphics.FromImage(bmp))
                {
                    requiredSize = InternalSize(g, _map);
                }
                requiredSize = Size.Add(Size.Add(BorderMargin, BorderMargin), requiredSize);
                requiredSize = Size.Add(Size.Add(BorderMargin, BorderMargin), requiredSize);
                requiredSize = Size.Add(new Size(BorderWidth, BorderWidth), requiredSize);
            }
            if (requiredSize.Width <= 0 || requiredSize.Height <= 0)
                return null;

            var res = new Bitmap(requiredSize.Width, requiredSize.Height);
            res.SetResolution(dpi,dpi);

            using (var g = Graphics.FromImage(res))
            {
            	g.TranslateTransform(0.5f*BorderWidth + BorderMargin.Width, 0.5f*BorderWidth + BorderMargin.Height);
            	g.Clear(Color.White);
                Render(g, _map);
            }

            Location = location;
            Anchor = anchor;

            return res;
        }

    }
}