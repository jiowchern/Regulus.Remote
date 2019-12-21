namespace Regulus.Remote
{

    public interface IBinderProvider
    {
        /// <summary>
        ///     如果客戶端連線成功系統會呼叫此方法並把Binder傳入。
        /// </summary>
        /// <param name="binder"></param>
        void AssignBinder(IBinder binder);
    }
}
