using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Extension
{
    public static class Linq
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> set)
        {

            return set.OrderBy( (d) => Regulus.Utility.Random.Instance.NextDouble());
        }
    }
}
