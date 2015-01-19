using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

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
