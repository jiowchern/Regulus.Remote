using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Hall : Regulus.Project.TurnBasedRPG.UserRoster , Regulus.Game.IFramework
    {        
        private Regulus.Game.FrameworkRoot _FrameworkRoot;

        public Hall()
        {            
            _FrameworkRoot = new Regulus.Game.FrameworkRoot();
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

        void Regulus.Game.IFramework.Launch()
        {
            
        }

        bool Regulus.Game.IFramework.Update()
        {
            _FrameworkRoot.Update();
            return true;
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            
        }
    }
}
