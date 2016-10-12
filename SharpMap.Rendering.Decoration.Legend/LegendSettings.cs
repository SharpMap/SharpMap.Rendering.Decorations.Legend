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

namespace SharpMap.Rendering.Decoration.Legend
{
    public class LegendSettings : ILegendSettings
    {
        /// <summary>
        /// Creates an instance of this class with default values
        /// </summary>
        public LegendSettings()
            :this(new Font("Arial", 16f, FontStyle.Bold),
                  new Font("Arial", 12f, FontStyle.Regular),
                  new Size(16, 16), new Size(3, 3))
        {}
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="headerFont"></param>
        /// <param name="itemFont"></param>
        /// <param name="symbolSize"></param>
        /// <param name="padding"></param>
        public LegendSettings(Font headerFont, Font itemFont, Size symbolSize, Size padding)
        {
            ForeColor = Brushes.Black;
            HeaderFont = headerFont;
            ItemFont = itemFont;
            SymbolSize = symbolSize;
            Indentation = SymbolSize.Width;
            Padding = padding;
        }

        /// <summary>
        /// The forecolor of the header fonts
        /// </summary>
        public Brush ForeColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default header font
        /// </summary>
        public Font HeaderFont { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default item font
        /// </summary>
        public Font ItemFont { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default indentation value
        /// </summary>
        public int Indentation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default padding between items
        /// </summary>
        public Size Padding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default symbol size
        /// </summary>
        public Size SymbolSize { get; set; }
    }
}