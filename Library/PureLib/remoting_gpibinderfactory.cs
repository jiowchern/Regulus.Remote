using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{

    public interface IGPIBinderFactory
    {
        GPIBinder<T> Create<T>(Regulus.Remoting.Ghost.INotifier<T> notice) where T : class;
    }
    public class GPIBinderFactory : IGPIBinderFactory
    {
        struct Data
        {            
            public IGPIBinder Binder;
        }
        List<Data> _Binders;
        Regulus.Utility.Command _Command;
        public GPIBinderFactory(Regulus.Utility.Command command)
        {
            _Command = command;
            _Binders = new List<Data>();
        }
        public void Setup()
        {
            foreach(var binder in  _Binders)
            {
                binder.Binder.Launch();
            }
            
        }


        public void Remove()
        {
            foreach(var binder in  _Binders)
            {
                binder.Binder.Shutdown();
            }

            _Binders.Clear();
        }

        public GPIBinder<T> Create<T>(Regulus.Remoting.Ghost.INotifier<T> notice) where T : class
        {
            var binder = new GPIBinder<T>(notice , _Command);
            _Binders.Add( new Data { Binder = binder });
            return binder ;
        }
        
    }
}
