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

using System.Drawing;
using SharpMap.Rendering.Decoration.Legend.Factories;

namespace SharpMap.Rendering.Decoration.Legend
{
    /// <summary>
    /// Interface for legend objects
    /// </summary>
    public interface ILegend : IMapDecoration
    {
        /// <summary>
        /// Gets or sets the root item of the legend
        /// </summary>
        ILegendItem Root { get; }

        /// <summary>
        /// Gets the factory that has been used to create this legend
        /// </summary>
        ILegendFactory Factory { get; }

        /// <summary>
        /// Method to get an image of the legend
        /// </summary>
        /// <param name="dpi">The dots-per-inch value</param>
        /// <returns>An image</returns>
        Image GetLegendImage(int dpi = 96);
    }
}