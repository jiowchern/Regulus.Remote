using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    internal class Game :Regulus.Game.ICore
    {

        Regulus.Utility.Updater _Update;
        Zone _Zone;
        Regulus.Game.Hall _Hall;
        IStorage _Storage;

        public Game(IStorage stroage)
        {
            _Zone = new Zone();
            _Storage = stroage;
            _Update = new Utility.Updater();
        }
        void Regulus.Game.ICore.ObtainController(Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder, _Storage, _Zone));
        }

        bool Utility.IUpdatable.Update()
        {
            _Update.Update();

            
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Update.Add(_Hall);
            _Update.Add(_Zone);
        }

        void Framework.ILaunched.Shutdown()
        {
            _Update.Shutdown();
        }
    }
}
