using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public interface IOnline
    {        
        float Ping { get; }
        void Disconnect();

        event Action DisconnectEvent;
    }


    public class Online : IOnline, Regulus.Remoting.Ghost.IGhost
    {
        Guid _Id;

        public Guid Id { get { return _Id;  } }        
        
        public Online()
        {
            _Id = Guid.NewGuid();
        }

        public Online(Remoting.IAgent agent) : this()
        {            
            this._Agent = agent;
            
        }
        float IOnline.Ping
        {
            get { return _Agent.Ping; }
        }

        void IOnline.Disconnect()
        {
            _Agent.Disconnect();            
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
        
        private Remoting.IAgent _Agent;
        event Action IOnline.DisconnectEvent
        {
            add { _Agent.DisconnectEvent += value; }
            remove { _Agent.DisconnectEvent -= value; }
        }        
        
    }


}
