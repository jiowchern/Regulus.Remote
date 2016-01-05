using System;


using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula
{
	public interface IUser : IUpdatable
	{
		event Action VersionErrorEvent;

		Regulus.Remoting.User Remoting { get; }

		INotifier<IVerify> VerifyProvider { get; }

		INotifier<IFishStageQueryer> FishStageQueryerProvider { get; }

        /// <summary>
        /// 錯誤的方法呼叫
        /// 如果呼叫的方法參數有誤則會回傳此訊息.
        /// 事件參數:
        ///     1.方法名稱
        ///     2.錯誤訊息
        /// 會發生此訊息通常是因為client與server版本不相容所致.
        /// </summary>
	    event Action<string, string> ErrorMethodEvent;
    }
}
