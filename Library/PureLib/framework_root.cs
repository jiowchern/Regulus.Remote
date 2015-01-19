using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

namespace Regulus.Framework
{
    public class Root<TUser>
    {

        System.Collections.Generic.List<TUser> _Users;

        IUserFactoty<TUser> _Provider;

        public Root(IUserFactoty<TUser> provider)
        {            
            _Provider = provider;
            _Users = new List<TUser>();
        }
        public TUser Spawn()
        {
            var user = _Provider.SpawnUser();
            _Users.Add(user);
            return user;
        }

        public void Unspawn(TUser user)
        {
            _Users.Remove(user);
        }
    }
}
