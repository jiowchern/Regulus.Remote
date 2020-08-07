namespace Regulus.Extension
{
    public static class QueueThreadHelper
    {
        public static int SafeCount<T>(this System.Collections.Generic.Queue<T> queue)
        {
            lock (queue)
            {
                return queue.Count;
            }
        }
        public static void SafeEnqueue<T>(this System.Collections.Generic.Queue<T> queue, T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
            }
        }

        public static T SafeDequeue<T>(this System.Collections.Generic.Queue<T> queue)
        {
            T item = default(T);
            lock (queue)
            {
                if (queue.Count > 0)
                    item = queue.Dequeue();
            }
            return item;
        }
    }
}