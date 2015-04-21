using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class CommandParser : Regulus.Framework.ICommandParsable<IUser>
    {
        void Regulus.Framework.ICommandParsable<IUser>.Setup(Regulus.Remoting.IGPIBinderFactory build)
        {
            
        }

        void Regulus.Framework.ICommandParsable<IUser>.Clear()
        {
            
        }
    }
}
