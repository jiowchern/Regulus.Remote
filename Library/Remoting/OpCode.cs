namespace Regulus.Remoting
{
	public enum ClientToServerOpCode
	{
		CallMethod = 1, 

		Ping, 

		Release
	};

	public enum ServerToClientOpCode
	{
		InvokeEvent = 1, 

		LoadSoul, 

		UnloadSoul, 

		ReturnValue, 

		UpdateProperty, 

		LoadSoulCompile, 

		Ping,

	    ErrorMethod
	}
}
