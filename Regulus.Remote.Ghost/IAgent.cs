using Regulus.Network;
using System;

namespace Regulus.Remote.Ghost
{

    /// <summary>
    ///     代理器
    /// </summary>
    /// 

    public interface IAgent : INotifierQueryable
    {


        /// <summary>
        ///     Ping
        /// </summary>
        long Ping { get; }


        /// <summary>
        /// 錯誤的方法呼叫
        /// 如果呼叫的方法參數有誤則會回傳此訊息.
        /// 事件參數:
        ///     1.方法名稱
        ///     2.錯誤訊息
        /// 會發生此訊息通常是因為client與server版本不相容所致.
        /// </summary>
        event Action<string, string> ErrorMethodEvent;


        /// <summary>
        /// 驗證錯誤
        /// 代表與伺服器端的驗證碼不符
        /// 事件參數:
        ///     1.伺服器驗證碼
        ///     2.本地驗證碼
        /// 會發生此訊息通常是因為client與server版本不相容所致.
        /// </summary>
        event Action<byte[], byte[]> ErrorVerifyEvent;


        void Start(IStreamable peer);
        void Stop();

        void Update();
    }
}