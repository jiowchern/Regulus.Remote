using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    public interface ICommandParsable<T>
    {
        void Unregister(T user, Utility.Command command);

        void Register(T user, Utility.Command _Command);
    }
}
