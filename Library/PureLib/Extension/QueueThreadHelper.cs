namespace Regulus.Extension
{
    public static class QueueThreadHelper
    {
        public static  void SafeEnqueue<T>(this System.Collections.Generic.Queue<T> queue , T item)
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
                item = queue.Dequeue();
            }
            return item;
        }
    }
}