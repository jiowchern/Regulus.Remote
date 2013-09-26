using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class ConsoleViewer : Console.IViewer
    {

        void Console.IViewer.WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }


        void Console.IViewer.Write(string message)
        {
            System.Console.Write(message);
        }
    }
}
