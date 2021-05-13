


namespace Regulus.Network
{
    public static class TaskExtension
    {
        public static IWaitableValue<T> ToWaitableValue<T>(this System.Threading.Tasks.Task<T> task)
        {
            return new TaskWaitableValue<T>(task);
        }

        public static IWaitableValue<T> ToWaitableValue<T>(this T val)
        {
            return new NoWaitValue<T>(val);
        }
    }
}