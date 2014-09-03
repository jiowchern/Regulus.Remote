using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts.Data
{

    [ProtoBuf.ProtoContract]
    public struct Record
    {
        [ProtoBuf.ProtoMember(1)]
        public Guid Id;
    }
}
