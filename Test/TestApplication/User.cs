using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    class User : IUser
    {
        Regulus.Standalong.Agent _Agent;
        public User()
        {
            _Agent = new Regulus.Standalong.Agent();
        }
        public void Launch()
        {
            _Agent.Launch();
            
        }
        internal void Update()
        {
            _Agent.Update();
        }

        public void Shutdown()
        {
            _Agent.Shutdown();
        }



		bool Regulus.Utility.IUpdatable.Update()
		{
			throw new NotImplementedException();
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
