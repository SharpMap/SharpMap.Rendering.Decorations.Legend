// Copyright 2014 - Felix Obermaier (www.ivv-aachen.de)
//
// This file is part of SharpMap.Rendering.Decoration.Legend.
// SharpMap.Rendering.Decoration.Legend is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap.Rendering.Decoration.Legend is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
    
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace SharpMap.Rendering.Decoration.Legend
{
    /// <summary>
    /// Implementation of a legend item
    /// </summary>
    internal class LegendItem : /*MapDecoration,*/ ILegendItem
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        public LegendItem()
        {
            SubItems = new Collection<ILegendItem>();
            //Anchor = SharpMap.Rendering.Decoration.MapDecorationAnchor.LeftTop;
            //Location = Point.Empty;
        }
        
        Size ILegendItem.InternalSize(Graphics g, Map map)
        {
        	if (Exclude) return Size.Empty;

            var size = ComputeItemSize(g, map);
            foreach (var subItem in SubItems)
            {
                var subItemSize = subItem.InternalSize(g, map);
                subItemSize.Width += Indentation;
                if (subItem is MapDecoration)
                {
                    var tmp = (MapDecoration) subItem;
                    subItemSize =  Size.Add(subItemSize, Size.Add(tmp.BorderMargin, tmp.BorderMargin));
                }
                size = ComputeMergedSize(size, subItemSize);
            }
            return size;
        }

        public Size Padding { get; set; }
        
        /// <summary>
        /// Method to compute the merged size
        /// </summary>
        /// <param name="size1">First size</param>
        /// <param name="size2">Second size</param>
        /// <returns>The merged size</returns>
        private Size ComputeMergedSize(Size size1, Size size2)
        {
            var width = Math.Max(size1.Width, size2.Width);
            var height = size1.Height + Padding.Height + size2.Height;

            return new Size(width , height);
        }

        /// <summary>
        /// Method to compute the item size
        /// </summary>
        /// <param name="g"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        protected Size ComputeItemSize(Graphics g, Map map)
        {
            var width = 0;
            var height = 0;

            if (Symbol != null)
            {
                height = Math.Max(height, Symbol.Size.Height);
                width += Symbol.Size.Width + Padding.Width;
            }

            if (!string.IsNullOrEmpty(Label))
            {
                var labelSize = Size.Ceiling(VectorRenderer.SizeOfString(g, Label, LabelFont));
                height = Math.Max(height, labelSize.Height);
                width += labelSize.Width + Padding.Width;
            }
               
            return new Size(width > 0 ? width - Padding.Width : 0, height);
        }

        /// <summary>
        /// Gets or sets a value indicating if this item should be excluded from the appear
        /// </summary>
        public bool Exclude { get; set; }

        /// <summary>
        /// Gets or sets the image of the layer
        /// </summary>
        public virtual Image Symbol { get; set; }

        /// <summary>
        /// Gets or sets the label for the legend item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the font to write the <see cref="ILegendItem.Label"/>
        /// </summary>
        public Font LabelFont { get; set; }

        /// <summary>
        /// Gets or sets the brush to write the <see cref="ILegendItem.Label"/>
        /// </summary>
        public Brush LabelBrush { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the indentation for sub items
        /// </summary>
        public int Indentation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that <see cref="ILegendItem.SubItems"/> should be expanded
        /// </summary>
        public virtual bool Expanded { get; set; }

        /// <summary>
        /// Gets a collection of sub items. These can be of type <see cref="IMapDecoration"/>
        /// </summary>
        public ICollection<ILegendItem> SubItems { get; private set; }

        /// <summary>
        /// Function to render the actual map decoration
        /// </summary>
        /// <param name="g"/><param name="map"/>
        void IMapDecoration.Render(Graphics g, Map map)
        {
            // nothing to do if excluded
            if (Exclude) return;

            var itemSize = ComputeItemSize(g, map);
            var offset = Point.Empty;
            if (Symbol != null)
            {
            	g.DrawImage(Symbol, 
            	            new Rectangle(0, (int)(0.5f* (itemSize.Height - Symbol.Size.Height)), Symbol.Size.Width, Symbol.Size.Height),
            	            new Rectangle(0, 0, Symbol.Size.Width,Symbol.Size.Height), GraphicsUnit.Pixel);
                offset.X += Symbol.Size.Width + Padding.Width;
            }

            if (!string.IsNullOrEmpty(Label))
            {
                var rect = new RectangleF(offset.X, 0, itemSize.Width - offset.X, itemSize.Height);
                g.DrawString(Label, LabelFont, LabelBrush, rect, new StringFormat()
                { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            }

            if (Expanded)
            {
                // Render sub - items
                g.TranslateTransform(Indentation, itemSize.Height + Padding.Height);
                foreach (var legendItem in SubItems)
                {
                    legendItem.Render(g, map);
                }
                g.TranslateTransform(-Indentation, Padding.Height);
            }
            else
            {
                g.TranslateTransform(0, itemSize.Height + Padding.Height);
            }

        }
        
        
    }
}