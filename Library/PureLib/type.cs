using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{

    [Serializable]
    public class Rect
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
    }
    [Serializable]
    public class Vector2
    {
        public Vector2()
        {
            X = 0.0f;
            Y = 0.0f;
        }
        public float X,Y;
    }
}
