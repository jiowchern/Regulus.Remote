using System;

namespace Regulus.Remote
{
    internal interface ISocketReader : Regulus.Utility.IBootable
    {
        event OnByteDataCallback DoneEvent;
        event OnErrorCallback ErrorEvent;
        
    }
}