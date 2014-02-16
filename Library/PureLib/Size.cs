using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    public struct Size
    {


        public Size(float width, float height)
        {            
            Width = width;
            Height = height;
        }

        public float Height;

        public float Width;
    }
}
