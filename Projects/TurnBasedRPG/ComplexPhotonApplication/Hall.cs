using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Hall : Regulus.Project.TurnBasedRPG.UserRoster , Samebest.Game.IFramework
    {        
        private Samebest.Game.FrameworkRoot _FrameworkRoot;

        public Hall()
        {            
            _FrameworkRoot = new Samebest.Game.FrameworkRoot();
        }            
        internal void PushUser(User user)
        {
            user.QuitEvent += () => 
            {                
                _FrameworkRoot.RemoveFramework(user);
                _User.Remove(user);
            };
            _FrameworkRoot.AddFramework(user);
            _User.Add(user);
        }

        void Samebest.Game.IFramework.Launch()
        {
            
        }

        bool Samebest.Game.IFramework.Update()
        {
            _FrameworkRoot.Update();
            return true;
        }

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }
}
