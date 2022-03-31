namespace Regulus.Remote.Tools.Protocol.Sources
{
    public static class HastSetExtensions
    {
        public static int AddRange<T>(this System.Collections.Generic.HashSet<T> set , System.Collections.Generic.IEnumerable<T> items)
        {
            int count = 0;
            foreach (var item in items)
            {
                if (set.Add(item))
                    count++;
            }

            return count;
        }
    }
}
