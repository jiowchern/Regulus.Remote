using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public interface IRealmJumper
    {
        
        Regulus.Remoting.Value<string[]> Query();
        void Jump(string realm);

        void Quit();
    }
}
