using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Extension
{
    public static class Dictionary
    {
        public static IEnumerator<Value> Values<Key,Value>(this System.Collections.Generic.Dictionary<Key, Value> datas)
        {
            foreach (var data in datas)
                yield return data.Value;
        }

        public static IEnumerator<Key> Keys<Key, Value>(this System.Collections.Generic.Dictionary<Key, Value> datas)
        {
            foreach (var data in datas)
                yield return data.Key;
        }
        
    }
}
