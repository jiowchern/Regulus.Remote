using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Imdgame.RunLocusts
{
    public class Game  : Regulus.Game.ICore
    {
        Hall _Hall;
       

        void Regulus.Game.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder));
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Hall = new Hall();
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
