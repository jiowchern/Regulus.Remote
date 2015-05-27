using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{

    /// <summary>
    /// 伺服器端對外綁定物件
    /// </summary>
	public interface ISoulBinder
	{
        /// <summary>
        /// 將介面物件傳回給客戶端，客戶端沒參考時則會自動釋放。
        /// </summary>
        /// <typeparam name="TSoul"></typeparam>
        /// <param name="soul"></param>
        void Return<TSoul>(TSoul soul);

        /// <summary>
        /// 將介面物件綁定給客戶端。
        /// </summary>
        /// <typeparam name="TSoul"></typeparam>
        /// <param name="soul"></param>
		void Bind<TSoul>(TSoul soul);

        /// <summary>
        /// 解綁定給客戶端的介面物件
        /// </summary>
        /// <typeparam name="TSoul"></typeparam>
        /// <param name="soul"></param>
		void Unbind<TSoul>(TSoul soul);

        /// <summary>
        /// 如果發生與客戶端斷線則會發生此事件
        /// </summary>
		event Action BreakEvent;
	}
}
