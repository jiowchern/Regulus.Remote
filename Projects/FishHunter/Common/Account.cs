using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Data
{
    public class Account
    {
        public Guid Id;
        public string Name;
        public string Password;
        public bool IsPassword(string password)
        {
            return Password == password;
        }
    }
}
