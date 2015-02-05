using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IVerify
    {
        Regulus.Remoting.Value<bool> Login(string id , string password);
    }
}
