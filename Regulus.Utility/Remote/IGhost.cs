using System;
using System.Reflection;

namespace Regulus.Remote
{
    public delegate void CallMethodCallback(MethodInfo info , object[] args ,IValue return_value);
	public delegate void EventNotifyCallback(EventInfo info , long handler_id);

	public interface IGhost
	{


		long GetID();

	    object GetInstance();

		bool IsReturnType();

        event CallMethodCallback CallMethodEvent;
		event EventNotifyCallback AddEventEvent;
		event EventNotifyCallback RemoveEventEvent;
	}
}
