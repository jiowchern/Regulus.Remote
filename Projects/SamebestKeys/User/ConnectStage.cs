using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys
{
    public class ConnectStage : Regulus.Utility.IStage
    {
        public delegate Regulus.Remoting.Value<bool> OnConnect(string ipaddr, int poirt);        
        Connect _Connecter;
        public event Action<bool> ResultEvent;
        private Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect> _ConnectProvider;
        OnConnect _Connect;

        public ConnectStage(Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect> connect_provider, OnConnect connect)
        {
            // TODO: Complete member initialization
            this._ConnectProvider = connect_provider;
            _Connect = connect;
            _Connecter = new Connect();
        }

        void Utility.IStage.Enter()
        {
            _Connecter.ConnectedEvent += _OnConnect;
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Add(_Connecter);
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Ready(_Connecter.Id);
        }

        void _OnConnect(string ipaddr, int port, Regulus.Remoting.Value<bool> result)
        {
            var value = _Connect(ipaddr , port);
            value.OnValue += (ret) =>
            {
                result.SetValue(ret);
                ResultEvent(ret);
            };
        }
        void Utility.IStage.Leave()
        {
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Remove(_Connecter.Id);
        }

        void Utility.IStage.Update()
        {

        }
    }
    
}
