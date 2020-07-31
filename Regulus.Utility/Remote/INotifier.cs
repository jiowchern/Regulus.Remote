using System;

namespace Regulus.Remote
{
    /// <summary>
    ///     介面物件通知器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INotifier<T> 
	{		

		/// <summary>
		///     伺服器端如果有物件傳入則會發生此事件
		/// </summary>
		event Action<T> Supply;

		/// <summary>
		///     伺服器端如果有物件關閉則會發生此事件
		/// </summary>
		event Action<T> Unsupply;

		/// <summary>
		///     在系統裡的介面物件數量
		/// </summary>
		T[] Ghosts { get; }

		
	}
}
