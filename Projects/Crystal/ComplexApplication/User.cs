using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Regulus.Project.Crystal
{
    class User : Samebest.Game.IFramework
    {
        public  Samebest.Remoting.Soul.SoulProvider Provider {get ; private set;}
		
        Samebest.Game.StageMachine<User> _Machine ;
		
		public User(Samebest.Remoting.Soul.SoulProvider provider)
		{			
			Provider = provider;		
			_Machine = new Samebest.Game.StageMachine<User>(this);			
			Provider.BreakEvent += _OnInactive;
		}

		void _OnInactive()
		{
			if (InactiveEvent != null)
				InactiveEvent();
		}
        
        void Samebest.Game.IFramework.Launch()
        {
			ToVerify();						
        }

		private void ToVerify()
		{
			_Machine.Push(new VerifyStage());
		}

        bool Samebest.Game.IFramework.Update()
        {
			
			_Machine.Update();
			return false;
        }

        void Samebest.Game.IFramework.Shutdown()
        {			
			_Machine.Termination();    
        }

        ~User()
		{

		}

		public event Action InactiveEvent;
    }
}
