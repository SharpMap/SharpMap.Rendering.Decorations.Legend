using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpMap.Rendering.Decoration.Legend.UI
{
    internal class LegendHitTester
    {
        private readonly ILegend _legend;
        private readonly Panel _panel;

        public LegendHitTester(ILegend legend, Panel panel)
        {
            _legend = legend;
            _panel = panel;
        }

        public HitTestResult HitTest(Point pt)
        {
            if (_legend == null)
                return null;

            var ls = (LegendSettings) _legend.Settings;
            var ptOffset = GetOffset(_legend);

            if (pt.X < ptOffset.X || pt.X > _panel.Width - ptOffset.X)
                return new HitTestResult(null, HitTestArea.None);

            if (pt.Y < ptOffset.Y || pt.Y > _panel.Height - ptOffset.Y)
                return new HitTestResult(null, HitTestArea.None);

            var g = _panel.CreateGraphics();

            var iter = new LegendItemEnumerator(_legend.Root);
            ILegendItem legendItem = null;
            var itmSize = Size.Empty;
            var y = ptOffset.Y;
            while (iter.MoveNext())
            {
                var itm = iter.Current;
                itmSize = itm.ComputeItemSize(g);
                if (y <= pt.Y && pt.Y < y + itmSize.Height)
                {
                    legendItem = itm;
                    break;
                }
                y += itmSize.Height + 2 * itm.Padding.Height;
            }

            if (legendItem != null)
            {
                var indentation = GetIndentation(legendItem);
                pt.X -= ptOffset.X;
                if (pt.X >= indentation)
                {
                    var x = pt.X - indentation;
                    if (legendItem.Symbol != null) {
                        if (x <= legendItem.Symbol.Width)
                            return new HitTestResult(legendItem, HitTestArea.Symbol);
                        x -= legendItem.Symbol.Width - legendItem.Padding.Width;
                    }
                    var labelSize = g.MeasureString(legendItem.Label, legendItem.LabelFont);
                    if (x <= labelSize.Width)
                        return new HitTestResult(legendItem, HitTestArea.Label);
                    x -= (int)Math.Ceiling(labelSize.Width - legendItem.Padding.Width);
                }
            }
            return new HitTestResult(null, HitTestArea.None);
        }

        private static int GetIndentation(ILegendItem legendItem)
        {
            var indentation = 0;
            var li = legendItem.Parent;
            while (li != null)
            {
                indentation += li.Indentation;
                li = li.Parent;
            }
            return indentation;
            throw new NotImplementedException();
        }

        private Point GetOffset(ILegend legend)
        {
            //var offset = new Point(0.5f * legend.BorderWidth + BorderMargin.Width, 0.5f * BorderWidth + BorderMargin.Height
            if (legend is Legend)
            {
                var l = (Legend) legend;
                return new Point(2 * l.BorderMargin.Width + (int)(0.5f * l.BorderWidth) /*+ l.Padding.Width*/, 
                                 2 * l.BorderMargin.Height + (int)(0.5f * l.BorderWidth) /*+ l.Padding.Height*/);
            }
            return new Point(legend.Settings.Padding);

        }
    }
}