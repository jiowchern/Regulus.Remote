using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{

    public interface IVerify
    {
        Regulus.Remoting.Value<Data.VerifyResult> Login(string account , string password);
    }

    
}
