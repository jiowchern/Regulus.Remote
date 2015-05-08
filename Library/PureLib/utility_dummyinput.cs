using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class DummyInput : Regulus.Utility.Console.IInput 
    {
        event Console.OnOutput Console.IInput.OutputEvent
        {
            add {  }
            remove {  }
        }
    }
}
