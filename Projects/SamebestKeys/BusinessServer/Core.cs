using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServer
{
    public delegate void OnVerifySucess (int account_id);
    public class Core : Regulus.Utility.ICore
    {
        Storage _Storage;
        Regulus.Utility.TUpdater<User> _Hall;
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            var user = new User(binder, _Storage);
            user.VerifySuccessEvent += _Relogin;
            _Hall.Add(user);
        }

        private void _Relogin(int account_id)
        {
            var users = from u in _Hall.Objects where u.Id == account_id select u;
            foreach(var user in users)
            {
                _Hall.Remove(user);
            }
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Hall.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Hall = new Regulus.Utility.TUpdater<User>();
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Hall.Shutdown();
        }

    
    }
}
