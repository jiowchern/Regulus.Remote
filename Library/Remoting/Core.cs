using Regulus.Utility;

namespace Regulus.Remote
{
	/// <summary>
	///     遠端物件提供的核心，欲提供給客戶端的物件主要進入點。
	/// </summary>
	public interface ICore : IBinderProvider
	{

        void Launch(IProtocol protocol , ICommand command);


	    


	    void Shutdown();
	}

    public interface IBinderProvider
    {
        /// <summary>
        ///     如果客戶端連線成功系統會呼叫此方法並把SoulBinder傳入。
        /// </summary>
        /// <param name="binder"></param>
        void AssignBinder(ISoulBinder binder);
    }
}
