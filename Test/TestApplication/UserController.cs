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


        void Regulus.Game.ConsoleFramework<IUser>.IController.Look()
        {
            throw new NotImplementedException();
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.NotLook()
        {
            throw new NotImplementedException();
        }


        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUserFail Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnFailEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

		bool Regulus.Utility.IUpdatable.Update()
		{
			_User.Update();
			return true;
		}

		void Regulus.Framework.ILaunched.Launch()
		{
			throw new NotImplementedException();
		}

		void Regulus.Framework.ILaunched.Shutdown()
		{
			throw new NotImplementedException();
		}
	}
}
