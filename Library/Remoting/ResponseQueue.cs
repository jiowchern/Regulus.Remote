using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public interface IResponseQueue
    {        
        void Push(byte cmd, Dictionary<byte, object> args);        
    }
}
