namespace Regulus.Remote
{
    public interface INotifierQueryable
    {
        /// <summary>
        ///     查詢介面物件通知者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        INotifier<T> QueryNotifier<T>();
    }
}