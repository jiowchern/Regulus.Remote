using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{
    using Extension;
    public interface IGPIBinder : Regulus.Framework.ILaunched
    {
    }


    public class GPIBinder<T> : IGPIBinder
    {
        
        struct Data
        {
            public Func<T, CommandParam> Handler;
            public string Name;
        }
        Regulus.Utility.Command _Command;
        List<Data> _Handlers;
        Regulus.Remoting.Ghost.IProviderNotice<T> _Notice;
        public GPIBinder(Regulus.Remoting.Ghost.IProviderNotice<T> notice , Regulus.Utility.Command command)
        {
            _Notice = notice;
            _Command = command;
            _Handlers = new List<Data>();
        }
        public void Add( string name , Func<T , CommandParam> handler)
        {
            _Handlers.Add(new Data { Handler = handler, Name = name });
        }

        void Framework.ILaunched.Launch()
        {
            _Notice.Supply += _Notice_Supply;        
        }

        void Framework.ILaunched.Shutdown()
        {
            _Notice.Supply -= _Notice_Supply; 
            foreach(var handler in  _Handlers)
            {
                _Command.Unregister(handler.Name);                
            }
        }

        void _Notice_Supply(T obj)
        {
            foreach(var handler in  _Handlers)
            {
                var param = handler.Handler(obj);
                _Command.Register(handler.Name, param);
                
            }
        }

        
    }
}
