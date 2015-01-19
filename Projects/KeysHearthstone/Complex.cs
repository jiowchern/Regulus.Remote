using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.KeysHearthstone
{
    public class Complex : Regulus.Utility.ICore
    {
		Regulus.Project.KeysHearthstone.Hall _Hall;
		Regulus.Utility.Updater _Updater;
		IStorage _Stroage;
		public Complex()
		{
			_Stroage = null;
			_Updater = new Utility.Updater();
			_Hall = new Hall();
		}
		void Utility.ICore.ObtainController(Remoting.ISoulBinder binder)
		{
			_Hall.PushUser(new User(binder, _Stroage));
		}

		bool Utility.IUpdatable.Update()
		{
			_Updater.Update();
			return true;
		}

		void Framework.ILaunched.Launch()
		{
			_Updater.Add(_Hall);
		}

		void Framework.ILaunched.Shutdown()
		{
			_Updater.Remove(_Hall);
			_Updater.Shutdown();
		}
	}
}
