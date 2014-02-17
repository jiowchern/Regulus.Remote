using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestNativeUser
{
    public interface IUser : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.Ghost.IProviderNotice<TestNativeGameCore.IMessager> MessagerProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<TestNativeGameCore.IConnect> ConnectProvider { get; }
    }

    public class User : IUser
    {
        class Connect : Regulus.Remoting.Ghost.IGhost,  TestNativeGameCore.IConnect
        {
            Guid _Id;            
            public Guid Id{get {return _Id;}}

            public event Action<string, int, Regulus.Remoting.Value<bool> > ConnectedEvent;
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

            
            Regulus.Remoting.Value<bool> TestNativeGameCore.IConnect.Connect(string ipaddr, int port)
            {
                var val = new Regulus.Remoting.Value<bool>();
                ConnectedEvent(ipaddr, port, val);
                return val;
            }


            void Regulus.Remoting.Ghost.IGhost.OnProperty(string name, byte[] value)
            {
                throw new NotImplementedException();
            }
        }

        private Regulus.Remoting.Ghost.Native.Agent _Complex { get; set; }
        private Regulus.Remoting.IAgent _Agent { get { return _Complex; } }        

        Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _Updater;

        Connect _Connecter;

        Regulus.Remoting.Ghost.TProvider<TestNativeGameCore.IConnect> _ConnectProvider;

        public User()
        {
            _Updater = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<TestNativeGameCore.IConnect>();
            _Connecter = new Connect();
            _Complex = new Regulus.Remoting.Ghost.Native.Agent();            
        }
        Regulus.Remoting.Ghost.IProviderNotice<TestNativeGameCore.IMessager> IUser.MessagerProvider
        {
            get { return _Agent.QueryProvider<TestNativeGameCore.IMessager>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<TestNativeGameCore.IConnect> IUser.ConnectProvider
        {
            get { return _ConnectProvider; }
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Connecter.ConnectedEvent += _Connecter_ConnectedEvent;
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Add(_Connecter);
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Ready(_Connecter.Id);
        }

        private void _Connecter_ConnectedEvent(string addr, int port, Regulus.Remoting.Value<bool> result)
        {
 	        var val = _Complex.Connect(addr , port);
            val.OnValue += (r) => 
            {
                result.SetValue(r);
            };
            
            _Updater.Remove(_Complex);
            _Updater.Add(_Complex);
        }

        

        void Regulus.Framework.ILaunched.Shutdown()
        {
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Remove(_Connecter.Id);
        }

        
    }

}
