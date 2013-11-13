using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    class UserController :  Application.IController
    {
        User _User;
        string _Name;
        string Regulus.Game.ConsoleFramework<IUser>.IController.Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.Release()
        {
            
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.Initialize(Regulus.Utility.Console.IViewer view, Regulus.Utility.Command command)
        {
            
        }

        void Regulus.Game.IFramework.Launch()
        {
            
        }

        bool Regulus.Game.IFramework.Update()
        {
            _User.Update();
            return true;
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            
        }


        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserUnpawnEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}
