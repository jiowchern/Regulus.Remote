using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{
    public class CommandAutoBuild
    {
        struct Data
        {
            public Type BinderType;
            public IGPIBinder Binder;
        }
        List<Data> _Binders;
        Regulus.Utility.Command _Command;
        public CommandAutoBuild(Regulus.Utility.Command command)
        {
            _Command = command;
            _Binders = new List<Data>();
        }
        public GPIBinder<T> Add<T>(Regulus.Remoting.Ghost.IProviderNotice<T> notice)
        {
            var gpiBinder = new GPIBinder<T>(notice, _Command);
            var binder = new Data() 
            {
                BinderType = typeof(T),
                Binder = gpiBinder
            };
            binder.Binder.Launch();
            _Binders.Add(binder);
            return gpiBinder;
        }

        public void Remove<T>(Regulus.Remoting.Ghost.IProviderNotice<T> notice)
        {
            _Binders.RemoveAll( 
                (binder) => 
                    {
                        if(binder.BinderType == typeof(T))
                        {
                            binder.Binder.Shutdown();
                            return true;
                        }
                        return false;
                    });
        }
    }
}
