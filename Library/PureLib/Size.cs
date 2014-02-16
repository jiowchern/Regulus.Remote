using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    public struct Size
    {

        
        public Size(double width, double height)
        {            
            Width = width;
            Height = height;
        }

        public double Height;

        public double Width;
    }
}
