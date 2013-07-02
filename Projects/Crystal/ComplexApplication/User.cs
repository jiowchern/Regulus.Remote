using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Regulus.Project.Crystal
{
    class User : Samebest.Game.IFramework
    {
        
        IStorage _IStorage;
        public  Samebest.Remoting.Soul.SoulProvider Provider {get ; private set;}
		
        Samebest.Game.StageMachine<User> _Machine ;
		
		public User(Samebest.Remoting.Soul.SoulProvider provider , IStorage stroage)
		{			
			Provider = provider;		
			_Machine = new Samebest.Game.StageMachine<User>(this);			
			Provider.BreakEvent += _OnInactive;
            _IStorage = stroage;
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
            _Machine.Push(new VerifyStage(this , _IStorage));
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
        public void Quit()
        { 
            _OnInactive();
        }
		public event Action InactiveEvent;
    }
}
