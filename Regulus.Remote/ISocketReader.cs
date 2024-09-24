using System;

namespace Regulus.Remote
{
    internal interface ISocketReader 
    {        
        event OnErrorCallback ErrorEvent;        
    }
}