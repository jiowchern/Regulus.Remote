using System;

namespace Regulus.Remote
{
    internal interface ISocketReader : Regulus.Utility.IStatus
    {
        event OnByteDataCallback DoneEvent;
        event OnErrorCallback ErrorEvent;
        
    }
}