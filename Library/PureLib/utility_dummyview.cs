using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class DummyView : Regulus.Utility.Console.IViewer
    {
        void Console.IViewer.WriteLine(string message)
        {
            
        }

        void Console.IViewer.Write(string message)
        {
            
        }
    }
}
