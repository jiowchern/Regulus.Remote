using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class Ghost : Regulus.Remoting.Ghost.IGhost
    {
        Guid _Id;
        public Ghost()
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

        void Regulus.Remoting.Ghost.IGhost.OnProperty(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        bool Regulus.Remoting.Ghost.IGhost.IsReturnType()
        {
            throw new NotImplementedException();
        }
    }
}
