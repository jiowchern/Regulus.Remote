using System;

namespace Regulus.Remote
{

    public interface IBinderProvider
    { 
        /// <summary>
        ///     zh-tw:如果客戶端連線成功系統會呼叫此方法並把Binder傳入。
        ///     en:When the client is connected successfully, the system will call this method and pass the Binder.
        /// </summary>
        /// <param name="binder"></param>
        void AssignBinder(IBinder binder);

        
    }
}
