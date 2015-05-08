using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Data
{
    [ProtoBuf.ProtoContract]
    public class Record
    {
        [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Money { get; set; }


    }
}
