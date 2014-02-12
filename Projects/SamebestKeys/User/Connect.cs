using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys
{
    class Connect : Regulus.Remoting.Ghost.IGhost, Regulus.Project.SamebestKeys.IConnect
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

        void Regulus.Remoting.Ghost.IGhost.OnProperty(string name, object value)
        {
            throw new NotImplementedException();
        }
        

        Regulus.Remoting.Value<bool> Project.SamebestKeys.IConnect.Connect(string ipaddr, int port)
        {
            var val = new Regulus.Remoting.Value<bool>();
            ConnectedEvent(ipaddr, port, val);
            return val;
        }
    }
}
