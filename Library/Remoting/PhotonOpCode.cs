using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
	public enum ClientToServerPhotonOpCode
	{
		Ping,		
		CallMethod,		
	};

    public enum ServerToClientPhotonOpCode
    {
        InvokeEvent,
        LoadSoul,
        UnloadSoul,
        ReturnValue,
        UpdateProperty,
        LoadSoulCompile,
        Ping

    }
}
