using System;

namespace Regulus.Remote
{
    internal interface ISocketReader 
    {
        event OnByteDataCallback DoneEvent;
        event OnErrorCallback ErrorEvent;
        
    }
}