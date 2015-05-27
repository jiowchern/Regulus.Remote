using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{
    using Extension;
    public interface IGPIBinder   : Regulus.Framework.IBootable
    {        
    }

    

    public class GPIBinder<T> : IGPIBinder
        where T : class
    {

        public delegate CommandParam OnBuilder(T source);
        public delegate void OnSourceHandler(T source);

        public struct Data
        {
            OnBuilder _Builder;
            string _Name;
            public Data(string name, OnBuilder builder)
            {
                _Name = name;
                _Builder = builder;
            }
            public OnBuilder Builder { get { return _Builder; } }
            public string Name { get { return _Name; } }
        }

        public event OnSourceHandler SupplyEvent;
        public event OnSourceHandler UnsupplyEvent;
        Regulus.Utility.Command _Command;
        Regulus.Remoting.Ghost.INotifier<T> _Notice;
        
        struct Source
        {
            public int Sn;            
            public T GPI;
        }
        List<Source> _GPIs;
        List<Data> _Handlers;        
        public GPIBinder(Regulus.Remoting.Ghost.INotifier<T> notice ,Regulus.Utility.Command command )
        {
            _Command = command;
            _Notice = notice;            
            _GPIs = new List<Source>();
            _Handlers = new List<Data>();
        }

        void _Notice_Supply(T obj)
        {            
            int sn = _Checkin(obj);
            _Register(obj, sn);

            if (SupplyEvent != null)
                SupplyEvent(obj);                       
        }
        void _Notice_Unsupply(T obj)
        {
            if (UnsupplyEvent != null)
                UnsupplyEvent(obj);

            int sn = _Checkout(obj);
            _Unregister(sn);
        }

        private void _Register(T obj, int sn)
        {
            foreach (var handler in _Handlers)
            {
                var param = handler.Builder(obj);
                _Command.Register(_BuileName(sn, handler.Name), param);
            }
        }
        

        private void _Unregister(int sn)
        {
            foreach (var handler in _Handlers)
            {
                _Command.Unregister(_BuileName(sn , handler.Name));
            }
        }

        private int _Checkin(T obj)
        {
            int sn = _GetSn();
            _GPIs.Add(new Source { GPI = obj, Sn = sn });
            return sn ; 
        }

        private int _Checkout(T obj)
        {
            Source source = _Find(obj);
            _GPIs.Remove(source);
            return source.Sn;
        }

        private Source _Find(T obj)
        {
            return (from source in _GPIs where source.GPI == obj select source).SingleOrDefault();
        }

        int _Sn;
        private int _GetSn()
        {
            return _Sn++;
        }

        private static string _BuileName(int sn, string name)
        {
            return sn.ToString() + name;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Notice.Supply += _Notice_Supply;
            _Notice.Unsupply += _Notice_Unsupply;            
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Notice.Unsupply -= _Notice_Unsupply;
            _Notice.Supply -= _Notice_Supply;


            foreach (var gpi in _GPIs.ToArray())
            {
                _Notice_Unsupply(gpi.GPI);
            }
        }

        public void Bind( string name , OnBuilder builder)
        {
            _Handlers.Add(new Data(name, builder));
        }
    }
}
