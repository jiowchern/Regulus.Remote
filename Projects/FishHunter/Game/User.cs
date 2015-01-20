using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class User : Regulus.Game.IUser
    {
        
        void Regulus.Game.IUser.OnKick(Guid id)
        {
                        
        }

        event Regulus.Game.OnNewUser _VerifySuccessEvent;
        event Regulus.Game.OnNewUser Regulus.Game.IUser.VerifySuccessEvent
        {
            add { _VerifySuccessEvent += value; }
            remove { _VerifySuccessEvent -= value; }
        }

        event Regulus.Game.OnQuit _QuitEvent;
        event Regulus.Game.OnQuit Regulus.Game.IUser.QuitEvent
        {
            add { _QuitEvent += value; }
            remove { _QuitEvent -= value; }
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
