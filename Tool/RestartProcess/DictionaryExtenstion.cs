using System.Collections.Generic;
using System.Linq;
namespace Regulus.Project.RestartProcess
{
    static class DictionaryExtenstion
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> Unchecked<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            IEnumerable<TKey> keys)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var needs = dictionary.Keys;
            
            var uncheckKeys = from need in dictionary where keys.Any( n=> comparer.Equals(n,need.Key) ) == false select need;
            return uncheckKeys;
        }
    }
}