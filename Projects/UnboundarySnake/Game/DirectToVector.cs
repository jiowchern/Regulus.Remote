using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    class DirectToVectorAttribute : Attribute
    {
        public Vector Vector { get; private set; }
        public DirectToVectorAttribute(float x,float y)
        {
            this.Vector = new Vector() { X = x, Y = y };
            
        }
    }
}
