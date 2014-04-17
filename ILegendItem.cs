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
    
using System.Collections.Generic;
using System.Drawing;

namespace SharpMap.Rendering.Decoration.Legend
{
    /// <summary>
    /// Interface for items in <see cref="ILegend"/>.
    /// </summary>
    public interface ILegendItem : IMapDecoration
    {
        /// <summary>
        /// Gets or sets a value indicating if this item should be excluded from the appear
        /// </summary>
        bool Exclude { get; set; }
        
        /// <summary>
        /// Gets or sets the image of the layer
        /// </summary>
        Image Symbol { get; set; }

        /// <summary>
        /// Gets or sets the label for the legend item
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the font to write the <see cref="Label"/>
        /// </summary>
        Font LabelFont { get; set; }

        /// <summary>
        /// Gets or sets the brush to write the <see cref="Label"/>
        /// </summary>
        Brush LabelBrush { get; set; }

        /// <summary>
        /// Method to compute the internal size of the legend item
        /// </summary>
        /// <param name="g">The graphics object to render the legend item</param>
        /// <param name="map">The map</param>
        /// <returns></returns>
        Size InternalSize(Graphics g, Map map);

        /// <summary>
        /// Gets or sets the padding size
        /// </summary>
        Size Padding { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating the indentation for sub items
        /// </summary>
        int Indentation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that <see cref="SubItems"/> should be expanded
        /// </summary>
        bool Expanded { get; set; }

        /// <summary>
        /// Gets a collection of sub items. These can be of type <see cref="IMapDecoration"/>
        /// </summary>
        ICollection<ILegendItem> SubItems { get; }
    }
}
