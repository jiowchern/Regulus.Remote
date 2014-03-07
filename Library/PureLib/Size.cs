using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    [Serializable]
    [ProtoBuf.ProtoContract]
    public struct Size
    {


        public Size(float width, float height)
        {            
            Width = width;
            Height = height;
        }
        [ProtoBuf.ProtoMember(1)]
        public float Height;
        [ProtoBuf.ProtoMember(2)]
        public float Width;
    }
}
