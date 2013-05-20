using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class UserRoster 
    {
        protected List<User> _User = new List<User>();
        internal User Find(string name)
        {                
            foreach (var user in _User)
            {
                if (user.Name == name)
                    return user;
            }
            return null;
        }
    }
}
