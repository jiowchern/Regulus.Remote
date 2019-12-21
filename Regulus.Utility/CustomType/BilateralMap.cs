using System.Collections.Generic;

namespace Regulus.CustomType
{
    public class BilateralMap<TItem1, TItem2>
    {
        private readonly Dictionary<TItem1, TItem2> _Item1s;
        private readonly Dictionary<TItem2, TItem1> _Item2s;

        public BilateralMap()
        {
            _Item1s = new Dictionary<TItem1, TItem2>();
            _Item2s = new Dictionary<TItem2, TItem1>();
        }

        public BilateralMap(IEqualityComparer<TItem1> comparer1, IEqualityComparer<TItem2> comparer2)
        {
            _Item1s = new Dictionary<TItem1, TItem2>(comparer1);
            _Item2s = new Dictionary<TItem2, TItem1>(comparer2);
        }

        public bool TryGetItem2(TItem1 k1, out TItem2 k2)
        {
            return _Item1s.TryGetValue(k1, out k2);
        }

        public bool TryGetItem1(TItem2 k2, out TItem1 k1)
        {
            return _Item2s.TryGetValue(k2, out k1);
        }

        public void Add(TItem1 item1, TItem2 item2)
        {
            _Item1s.Add(item1, item2);
            _Item2s.Add(item2, item1);
        }
    }
}