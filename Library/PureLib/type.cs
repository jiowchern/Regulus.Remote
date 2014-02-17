using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{

    [ProtoBuf.ProtoContract]
    [Serializable]
    public class Vector2
    {
        public Vector2()
        {
            X = 0.0f;
            Y = 0.0f;
        }
        [ProtoBuf.ProtoMember(1)]
        public float X;
        [ProtoBuf.ProtoMember(2)]
        public float Y;
    }
}
