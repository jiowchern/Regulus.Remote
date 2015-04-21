using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class EmptyInputView : Regulus.Utility.Console.IInput, Regulus.Utility.Console.IViewer
    {
        void Regulus.Utility.Console.IViewer.WriteLine(string message)
        {
            
        }

        void Regulus.Utility.Console.IViewer.Write(string message)
        {
            
        }

        event Regulus.Utility.Console.OnOutput Regulus.Utility.Console.IInput.OutputEvent
        {
            add {  }
            remove {  }
        }
    }
}
