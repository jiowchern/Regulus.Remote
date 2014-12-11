using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    public interface IVerify
    {

        Regulus.Remoting.Value<VerifyResult> Login(string account , string password);
    }
}
