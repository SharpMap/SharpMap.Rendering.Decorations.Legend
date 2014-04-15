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

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    /// <summary>
    /// A factory to create a legend item
    /// </summary>
    public interface ILegendItemFactory
    {
        /// <summary>
        /// The type this factory is intended for
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Method to create the legend item
        /// </summary>
        /// <param name="legend">The legend a legend item should be created for</param>
        /// <param name="item">The item to create a legend item for</param>
        /// <returns>The legend item, if one could be created, otherwise <c>null</c></returns>
        ILegendItem Create(ILegend legend, object item);
    }
}