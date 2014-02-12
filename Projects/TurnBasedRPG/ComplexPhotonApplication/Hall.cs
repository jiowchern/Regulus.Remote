using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    
    class Hall : Regulus.Project.TurnBasedRPG.UserRoster , Regulus.Utility.IUpdatable
    {        
        private Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _FrameworkRoot;

        public Hall()
        {

			_FrameworkRoot = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
        }            
        internal void PushUser(User user)
        {
            user.QuitEvent += () => 
            {                
                _FrameworkRoot.Remove(user);
                _User.Remove(user);
            };
            _FrameworkRoot.Add(user);
            _User.Add(user);
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _FrameworkRoot.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
