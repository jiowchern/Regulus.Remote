using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{

    public interface ILaunched<T>
    {
        void Launch();
        void Shutdown();
    }
    public interface ILaunched
    {
        void Launch();
        void Shutdown();
    }
}
