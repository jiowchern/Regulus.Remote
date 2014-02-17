using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    [ProtoBuf.ProtoContract]
    public struct Point
    {


        public Point(float x, float y)
        {
            // TODO: Complete member initialization
            this.X = x;
            this.Y = y;
        }
        [ProtoBuf.ProtoMember(1)]
        public float X;
        [ProtoBuf.ProtoMember(2)]
        public float Y;
    }
}
