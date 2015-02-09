using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Formula
{
    public class Server : Regulus.Utility.ICore
    {
        VGame.Project.FishHunter.Formula.Center _Center;
        Regulus.Utility.ICore _Core { get { return _Center; } }
        public Server()
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
