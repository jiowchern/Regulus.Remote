using System;
using System.Collections.Generic;
using System.Linq;


namespace Regulus.Extension
{
    public static class Linq
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> set)
        {
            return set.OrderBy( (d) => Regulus.Utility.Random.Instance.NextDouble());
        }

        public static int Index<T>(this IEnumerable<T> set , Func<T,bool> condition)
        {
            var instance = condition;
            int index = 0;
            foreach (var item in set)
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
