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
        string Regulus.Utility.Framework<IUser>.IController.Name
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



        


        void Regulus.Utility.Framework<IUser>.IController.Look()
        {
            throw new NotImplementedException();
        }

        void Regulus.Utility.Framework<IUser>.IController.NotLook()
        {
            throw new NotImplementedException();
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


        IUser Regulus.Utility.Framework<IUser>.IController.GetUser()
        {
            throw new NotImplementedException();
        }
    }
}
