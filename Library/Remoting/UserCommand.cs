using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public interface IUserCommand
    {
        void Register();

        void Unregister();
    }
}
