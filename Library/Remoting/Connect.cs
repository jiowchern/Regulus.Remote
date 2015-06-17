using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public interface IConnect
    {        
        Regulus.Remoting.Value<bool> Connect(string ipaddr, int port);        
    }


    public class Connect : Regulus.Remoting.Ghost.IGhost, IConnect
    {
        Guid _Id;
        public Guid Id { get { return _Id; } }

        public event Action<string, int, Regulus.Remoting.Value<bool>> ConnectedEvent;
        public Connect()
        {
            _Id = Guid.NewGuid();
        }
        void Regulus.Remoting.Ghost.IGhost.OnEvent(string name_event, object[] args)
        {
            throw new NotImplementedException();
        }

        Guid Regulus.Remoting.Ghost.IGhost.GetID()
        {
            return _Id;
        }

        Regulus.Remoting.Value<bool> IConnect.Connect(string ipaddr, int port)
        {
            if (ConnectedEvent == null)
                throw new SystemException("Invalid Connect, to regain from the provider.");
            var val = new Regulus.Remoting.Value<bool>();
            ConnectedEvent(ipaddr, port, val);
            return val;
        }


        void Regulus.Remoting.Ghost.IGhost.OnProperty(string name, byte[] value)
        {
            throw new NotImplementedException();
        }





        bool Remoting.Ghost.IGhost.IsReturnType()
        {
            return false;
        }
    }

}
