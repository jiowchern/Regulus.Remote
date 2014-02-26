using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys.Standalong
{
    class Game : Regulus.Utility.Singleton<Game>, Regulus.Utility.IUpdatable
    {
        Regulus.Utility.Updater _Updater;
        Regulus.Game.ICore _Complex;
        Stroage _Stroage;
        int _UserCount;
        int _UpdateCount;
        public Game()
        {
            _Stroage = new Stroage();
            _Complex = new Regulus.Project.SamebestKeys.Complex(_Stroage);
            _Updater = new Utility.Updater();
        }

        public void Push(Regulus.Remoting.ISoulBinder binder)
        {
            binder.BreakEvent += () => { _UserCount--; };
            _Complex.ObtainController(binder);
            _UserCount++;
        }


        bool Utility.IUpdatable.Update()
        {
            if (_UserCount != 0 && ++_UpdateCount % _UserCount == 0)
            {
                _Complex.Update();
                _UpdateCount = 0;
            }

            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Complex.Launch();
        }

        void Framework.ILaunched.Shutdown()
        {
            _Complex.Shutdown();
        }
    }
}
