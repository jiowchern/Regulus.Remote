using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class LocalTime : Regulus.Utility.Singleton<Regulus.Remoting.Time>, Regulus.Utility.IUpdatable
    {

        void Regulus.Framework.ILaunched.Launch()
        {

        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            Instance.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {

        }
    }
}
