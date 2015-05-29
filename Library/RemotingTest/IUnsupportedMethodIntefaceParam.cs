using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    public interface IUnsupportedMethodIntefaceParam
    {
        void Function(IUnsupportedMethodIntefaceParam method);
    }
}
