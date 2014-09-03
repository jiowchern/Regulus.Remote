using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imdgame.RunLocusts
{
    public class Complex : Regulus.Game.ICore
    {
        Imdgame.RunLocusts.Game _Game;

        Regulus.Game.ICore _GameCore { get { return _Game; } }

        public Complex()
        {
            _Game = new Game();
        }
        void Regulus.Game.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {

            _GameCore.ObtainController(binder);
            
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            return _GameCore.Update();
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _GameCore.Launch();
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _GameCore.Shutdown();
        }
    }
}
