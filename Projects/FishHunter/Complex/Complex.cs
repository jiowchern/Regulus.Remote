using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter
{
    class Complex : Regulus.Utility.ICore
    {

        VGame.Project.FishHunter.Center _Center;
        Regulus.Utility.ICore _Core { get { return _Center; } }
        public Complex()
        {
            _Center = new Center();
        }

        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.ObtainController(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Core.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Core.Launch();
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Core.Shutdown();
        }
    }
}
