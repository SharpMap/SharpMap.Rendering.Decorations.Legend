using System.Diagnostics;

namespace SharpMap.Rendering.Decoration.Legend.UI
{
    public class HitTestResult
    {
        internal HitTestResult(ILegendItem item, HitTestArea hti)
        {
            Item = item;
            HitTestArea = hti;
        }

        public HitTestArea HitTestArea { get; private set; }

        public ILegendItem Item { get; private set; }

        public override string ToString()
        {
            if (Item != null)
                return string.Format("{0} item '{1}' at {2}", Item.GetType().Name, Item.Label, HitTestArea);
            return "No item hit.";
        }
    }
}