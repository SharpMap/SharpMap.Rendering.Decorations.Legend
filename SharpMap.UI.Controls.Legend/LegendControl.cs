/*
* Created by SharpDevelop.
* User: felix
* Date: 22.04.2014
* Time: 22:35
* 
* To change this template use Tools | Options | Coding | Edit Standard Headers.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SharpMap.Rendering.Decoration.Legend;
using SharpMap.Rendering.Decoration.Legend.Factories;

namespace SharpMap.Rendering.Decoration.Legend.UI
{
    /// <summary>
    /// Description of UserControl1.
    /// </summary>
    public partial class LegendControl : Control
    {
        ILegend _legend;
        ILegendFactory _legendFactory;
        Map _map;

        public event EventHandler MapChanged;

        protected virtual void OnMapChanged(EventArgs e)
        {
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
            var handler = LegendFactoryChanged;
            if (handler != null) handler(this, e);
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
        }

        public ILegendFactory Factory { get; set; }

        public ILegend Legend { get; private set; }

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


    }
}