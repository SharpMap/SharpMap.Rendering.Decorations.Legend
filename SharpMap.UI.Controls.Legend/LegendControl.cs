/*
* Created by SharpDevelop.
* User: felix
* Date: 22.04.2014
* Time: 22:35
* 
* To change this template use Tools | Options | Coding | Edit Standard Headers.
*/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using SharpMap.Rendering.Decoration.Legend;
using SharpMap.Rendering.Decoration.Legend.Factories;

namespace SharpMap.Rendering.Decoration.Legend.UI
{
    /// <summary>
    /// Description of UserControl1.
    /// </summary>
    public partial class LegendControl : ScrollableControl
    {
        private readonly Panel _legendPanel;
        private readonly TextBox _textBox;

        private ILegend _legend;
        private ILegendFactory _legendFactory;
        private Map _map;
        private ILegendSettings _legendSettings;

        public event EventHandler MapChanged;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        protected virtual void OnMapChanged(EventArgs e)
        {
            if (_map == null || _legendFactory == null)
                return;

            _legend = _legendFactory.Create(_map, _legendSettings);
            UpdateImage();


            var handler = MapChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler LegendChanged;

        protected virtual void OnLegendChanged(EventArgs e)
        {
            var handler = LegendChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler LegendFactoryChanged;

        protected virtual void OnLegendFactoryChanged(EventArgs e)
        {
            if (_map == null || _legendFactory == null)
                return;

            _legend = _legendFactory.Create(_map, _legendSettings);
            UpdateImage();

            var handler = LegendFactoryChanged;
            if (handler != null) handler(this, e);
        }

        private void UpdateImage()
        {
            var image = _legend.GetLegendImage();
            _legendPanel.AutoSize = false;
            _legendPanel.Size = image.Size;
            _legendPanel.BackgroundImage = image;
        }

        public LegendControl()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            _legendPanel = new Panel();
            _legendPanel.AutoSize = false;
            //_legendPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _legendPanel.Location = new Point(0, 0);
            _legendPanel.MouseDown += HandleMouseDown;
            _textBox = new TextBox();
            _textBox.Visible = false;
            
            Controls.Add(_legendPanel);
            Controls.Add(_textBox);

            base.AutoScroll = true;
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            var hitTester = new LegendHitTester(_legend, _legendPanel);
            var t = hitTester.HitTest(e.Location);
            Debug.WriteLine(t.ToString());
        }

        public ILegendFactory LegendFactory
        {
            get { return _legendFactory; }
            set
            {
                if (ReferenceEquals(_legendFactory, value))
                    return;
                _legendFactory = value;

                OnLegendFactoryChanged(EventArgs.Empty);
            }
        }

        public ILegend Legend { get { return _legend; } }

        public Map Map
        {
            get { return _map; }
            set
            {
                if (ReferenceEquals(_map, value))
                    return;
                _map = value;
                OnMapChanged(EventArgs.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var hitTester = new LegendHitTester(_legend, _legendPanel);
            var t = hitTester.HitTest(e.Location);

            switch (t.HitTestArea)
            {
                case HitTestArea.Label:
                case HitTestArea.Symbol:
                default:
                    Debug.WriteLine(t.ToString());
                    break;


            }

            base.OnMouseDown(e);
        }
    }
}