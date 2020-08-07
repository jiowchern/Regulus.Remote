using System;
using System.Collections.Generic;
using System.Linq;


namespace Regulus.Extension
{
    public static class Linq
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> set)
        {
            return set.OrderBy((d) => Regulus.Utility.Random.Instance.NextDouble());
        }

        public static int Index<T>(this IEnumerable<T> set, Func<T, bool> condition)
        {
            Func<T, bool> instance = condition;
            int index = 0;
            foreach (T item in set)
            {
                if (instance(item))
                {
                    return index;
                }
                index++;
            }
            throw new Exception("The container does not meet the conditions.");
        }
    }
}
