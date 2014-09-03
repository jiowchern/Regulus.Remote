using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts.Data
{
    [ProtoBuf.ProtoContract]
    public enum VerifyResult
    {
        Success,
        ErrorAccount
    }
}
