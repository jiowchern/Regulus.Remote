using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestNativeGameCore
{
    public interface IMessager
    {
        Regulus.Remoting.Value<string> Send(string message);

        
    }

    public interface IConnect
    {
        Regulus.Remoting.Value<bool> Connect(string ipaddr, int port);
    }
}
