using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public delegate void OnNewUser(Guid account);
    class Hall : Regulus.Utility.IUpdatable
    {
        public event OnNewUser NewUserEvent;
        private Regulus.Utility.Updater _Users;

        public Hall()
        {

			_Users = new Regulus.Utility.Updater();
        }            
        internal void PushUser(User user)
        {
            user.VerifySuccessEvent += (id) =>
            {
                if (NewUserEvent != null)
                    NewUserEvent(id);
                NewUserEvent += user.OnKick;
            };


            user.QuitEvent += () => 
            {
                NewUserEvent -= user.OnKick;
                _Users.Remove(user);                
            };
            _Users.Add(user);            
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Users.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
