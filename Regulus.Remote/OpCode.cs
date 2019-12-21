namespace Regulus.Remote
{
	public enum ClientToServerOpCode : byte
	{
		CallMethod = 1, 

		Ping, 

		Release
	};

	public enum ServerToClientOpCode : byte
	{
		InvokeEvent = 1, 

		LoadSoul, 

		UnloadSoul, 

		ReturnValue, 

		UpdateProperty, 

		LoadSoulCompile, 

		Ping,

	    ErrorMethod , 

        ProtocolSubmit
	}
}
