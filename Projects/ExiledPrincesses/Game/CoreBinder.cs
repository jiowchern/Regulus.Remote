using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public partial class Core 
    {
        public class PluralBinder<T> where T : class
        {
            private Remoting.ISoulBinder _Binder;
            T[] _Objects;
            public PluralBinder(Remoting.ISoulBinder binder, T[] objs)
            {
                _Binder = binder;
                _Objects = objs;
            }

            public void Differences(T[] source)
            {
                _Differences(_Objects, source);
                _Objects = source;
            }
            private void _Differences(T[] current, T[] actors)
            {
                var exits = current.Except(actors);
                foreach (var exit in exits)
                {
                    _Binder.Unbind<T>(exit);
                }

                var joins = actors.Except(current);
                foreach (var join in joins)
                {
                    _Binder.Bind<T>(join);
                }

            }
        }
        public class OnesBinder<T> where T : class
        {
            private Remoting.ISoulBinder _Binder;
            T _Object;
            public OnesBinder(Remoting.ISoulBinder binder)
            {
                _Binder = binder;
            }
            public void Set(T obj)
            {
                if (_Object != default(T))
                {
                    _Binder.Unbind<T>(_Object);
                    if (obj != default(T))
                        throw new SystemException("OnesBinder 重複綁定");
                }
                if (obj != default(T))
                    _Binder.Bind<T>(obj);
                _Object = obj;
            }
        }
    }
}
