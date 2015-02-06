using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{

    [Flags]
    [ProtoBuf.ProtoContract]
    public enum FISH_STATUS
    {
        NORMAL,
        KING,
        FREEZE
    }

    [ProtoBuf.ProtoContract]
    public struct HitRequest
    {
        [ProtoBuf.ProtoMember(1)]
        public short WepID;
        [ProtoBuf.ProtoMember(2)]
        public byte WepType;
        [ProtoBuf.ProtoMember(3)]
        public short WepBet;
        [ProtoBuf.ProtoMember(4)]
        public short WepOdds;
        [ProtoBuf.ProtoMember(5)]
        public short FishID;
        [ProtoBuf.ProtoMember(6)]
        public byte FishType;
        [ProtoBuf.ProtoMember(7)]
        public short FishOdds;
        [ProtoBuf.ProtoMember(8)]
        public FISH_STATUS FishStatus;
        [ProtoBuf.ProtoMember(9)]
        public byte TotalHits;
        [ProtoBuf.ProtoMember(10)]
        public byte HitCnt;
        [ProtoBuf.ProtoMember(11)]
        public short TotalHitOdds;
    }


    [ProtoBuf.ProtoContract]
    public struct HitResponse
    {
        [ProtoBuf.ProtoMember(1)]
        public short WepID;
        [ProtoBuf.ProtoMember(2)]
        public short FishID;
        [ProtoBuf.ProtoMember(3)]
        public byte DieResult;
        [ProtoBuf.ProtoMember(4)]
        public byte SpecAsn;
    }

    public delegate void HitResponseCallback(HitResponse response);
    public delegate void HitExceptionCallback(string message);
    public interface IFishStage
    {
        long AccountId { get; }
        byte FishStage { get; }
        void Hit(HitRequest request);

        event HitResponseCallback HitResponseEvent;
        event HitExceptionCallback HitExceptionEvent;
    }
}
