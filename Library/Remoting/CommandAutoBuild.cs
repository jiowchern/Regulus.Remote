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
        public GPIBinder<T> Add<T>(Regulus.Remoting.Ghost.IProviderNotice<T> notice)
        {
            var gpiBinder = new GPIBinder<T>(notice);
            var binder = new Data() 
            {
                BinderType = typeof(T),
                Binder = gpiBinder
            };
            _Binders.Add(binder);
            return gpiBinder;
        }

        public void Remove<T>(Regulus.Remoting.Ghost.IProviderNotice<T> notice)
        {
            _Binders.RemoveAll( (binder) => binder.BinderType == typeof(T));
        }
    }
}
