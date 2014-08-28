using System;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration.Legend.Properties;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    /// <summary>
    /// Abstract base implementation of a legend item factory
    /// </summary>
    public abstract class AbstractLegendItemFactory : ILegendItemFactory
    {
        private ILegendFactory _factory;

        /// <summary>
        /// Gets a value indicating the legend factory this legend item factory belongs to.
        /// </summary>
        public ILegendFactory Factory
        {
            get { return _factory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _factory = value;
            }
        }

        /// <summary>
        /// The type this factory is intended for
        /// </summary>
        public abstract Type[] ForType { get; }

        /// <summary>
        /// Method to create the legend item
        /// </summary>
        /// <param name="settings">The legend settings</param>
        /// <param name="item">The item to create a legend item for</param>
        /// <returns>The legend item, if one could be created, otherwise <c>null</c></returns>
        public virtual ILegendItem Create(ILegendSettings settings, object item)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");


            if (item == null)
                throw new ArgumentNullException("item");

            CheckType(item);

            var res = new LegendItem
            {
                Label = CreateLabel(item),
                Item = item,
                Symbol = CreateSymbol(settings.SymbolSize, item),
                LabelFont = settings.ItemFont,
                LabelBrush = settings.ForeColor,
                Padding = settings.Padding,
                Exclude = CheckExclude(item)
            };
            res.Expanded = CheckExpanded(res);
            return res;
        }

        private void CheckType(object item)
        {
            var itemType = item.GetType();
            foreach (var type in ForType)
            {
                if (type.IsAssignableFrom(itemType))
                    return;

                if (type.IsGenericTypeDefinition && itemType.IsGenericType)
                {
                    var gtd = itemType.GetGenericTypeDefinition();
                    if (gtd == type)
                        return;
                }
            }

            throw new ArgumentException(Resources.invalidType, "item");
        }

        protected virtual string CreateLabel(object item)
        {
            return string.Empty;
        }

        protected virtual Image CreateSymbol(Size symbolSize, object item)
        {
            return null;
        }

        protected virtual bool CheckExclude(object item)
        {
            return false;
        }

        protected virtual bool CheckExpanded(ILegendItem item)
        {
            return item.SubItems.Count > 0;
        }
    }

    public class DefaultLayerLegendItemFactory : AbstractLegendItemFactory
    {
        public override Type[] ForType
        {
            get { return new [] { typeof(ILayer) }; }
        }

        protected override Image CreateSymbol(Size symbolSize, object item)
        {
            var l = (ILayer) item;
            using (var m = new Map(symbolSize))
            {
                m.DisposeLayersOnDispose = false;
                m.Layers.Add(l);
                m.ZoomToBox(l.Envelope);
                return m.GetMap();
            }
        }

        protected override string CreateLabel(object item)
        {
            var l = (ILayer)item;
            return l.LayerName;
        }
    }
}