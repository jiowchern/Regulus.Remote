namespace Regulus.Utility
{
    public static class QueueThreadHelper
    {
        public static int SafeCount<T>(this System.Collections.Generic.Queue<T> Queue)
        {
            lock (Queue)
            {
                return Queue.Count;
            }
        }
        public static void SafeEnqueue<T>(this System.Collections.Generic.Queue<T> Queue, T Item)
        {
            lock (Queue)
            {
                Queue.Enqueue(Item);
            }
        }

        public static T SafeDequeue<T>(this System.Collections.Generic.Queue<T> Queue)
        {
            var item = default(T);
            lock (Queue)
            {
                if (Queue.Count > 0)
                    item = Queue.Dequeue();
            }
            return item;
        }
    }
}