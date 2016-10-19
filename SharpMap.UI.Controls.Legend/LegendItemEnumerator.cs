using System.Collections;
using System.Collections.Generic;

namespace SharpMap.Rendering.Decoration.Legend.UI
{
    public class LegendItemEnumerator : IEnumerator<ILegendItem>
    {
        private readonly ILegendItem _root;
        private readonly IEnumerator<ILegendItem> _internal;
        private ILegendItem _current;
        private IEnumerator<ILegendItem> _internalItems;

        public LegendItemEnumerator(ILegendItem root)
        {
            _root = root;
            if (root.Expanded && root.SubItems.Count > 0)
                _internal = root.SubItems.GetEnumerator();
        }


        public void Dispose()
        { }

        public bool MoveNext()
        {
            if (_current == null)
            {
                _current = _root;
                return !_current.Exclude || MoveNext();
            }

            if (Current == _root)
            {
                if (_internal == null) return false;
            }

            if (_internalItems != null)
            {
                if (_internalItems.MoveNext())
                {
                    while (_internalItems.Current.Exclude) {
                        if (!_internalItems.MoveNext()) break;
                    }

                    if (_internalItems.Current == null)
                        _internalItems = null;

                    return true;
                }
                /*
                if (_internalItems.Current != null)
                    return true;

                _internalItems = null;
                return false;
                */
            }

            if (_internal.MoveNext())
            {
                _internalItems = new LegendItemEnumerator(_internal.Current);
                return MoveNext();
            }

            _current = null;
            return false;
        }

        public void Reset()
        {
            _current = null;
            if (_internal != null) _internal.Reset();
            _internalItems = null;
        }

        public ILegendItem Current
        {
            get
            {
                return _internalItems != null && _internalItems.Current != null
                    ? _internalItems.Current
                    : _current;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}