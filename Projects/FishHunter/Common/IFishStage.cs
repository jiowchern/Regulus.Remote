using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    [Flags]
    [ProtoBuf.ProtoContract]
    public enum FISH_DETERMINATION
    {
        DEATH,
        SURVIVAL
    }

    /// <summary>
    /// 鱼的状态。
    /// </summary>
    [Flags]
    [ProtoBuf.ProtoContract]    
    public enum FISH_STATUS
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        NORMAL,
        /// <summary>
        /// 鱼王状态
        /// </summary>
        KING,
        /// <summary>
        /// 冰冻状态
        /// </summary>
        FREEZE
    }

    /// <summary>
    /// 判定請求
    /// </summary>
    [ProtoBuf.ProtoContract]
    public struct HitRequest
    {
        /// <summary>
        /// 子弹编号，识别是那颗子弹（网）用的。
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public short WepID;
        /// <summary>
        /// 子弹形态。一般子弹为“1”。特殊子弹从“2”开始。。。
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte WepType;
        /// <summary>
        /// 子弹分数，就是该子弹的玩家押注分数。从1~10000。
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public short WepBet;
        /// <summary>
        /// 子弹倍数，就是该子弹的玩家押注倍数。从1~10000。
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public short WepOdds;
        /// <summary>
        /// 鱼的编号，识别是那只鱼用的。
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public short FishID;
        /// <summary>
        /// 鱼的种类。一般鱼从1~99。特殊鱼从201开始。。。
        /// </summary>
        [ProtoBuf.ProtoMember(6)]
        public byte FishType;
        /// <summary>
        /// 鱼的倍数。应该从1~1000。目前只预留到1000倍。
        /// </summary>
        [ProtoBuf.ProtoMember(7)]
        public short FishOdds;
        /// <summary>
        /// 鱼的状态。         
        /// </summary>
        [ProtoBuf.ProtoMember(8)]
        public FISH_STATUS FishStatus;
        /// <summary>
        /// 该网同时击中的鱼总数量。应该从1~1000。
        /// </summary>
        [ProtoBuf.ProtoMember(9)]
        public short TotalHits;
        /// <summary>
        /// 同时击中的排序序号，应该是从1~ TotalHits。
        /// </summary>
        [ProtoBuf.ProtoMember(10)]
        public short HitCnt;
        /// <summary>
        /// 该网同时击中的鱼倍数总和。
        /// </summary>
        [ProtoBuf.ProtoMember(11)]
        public short TotalHitOdds;
    }



    /// <summary>
    /// 判定结果
    /// </summary>
    [ProtoBuf.ProtoContract]
    public struct HitResponse
    {
        /// <summary>
        /// 子弹编号
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public short WepID;
        /// <summary>
        /// 鱼的编号
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public short FishID;
        /// <summary>
        /// 死亡结果        
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public FISH_DETERMINATION DieResult;
        /// <summary>
        /// 取得特殊武器。
        /// 0表示没有取得。
        /// 有取得特殊武器从“2”开始。对应特武编号。
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public byte SpecAsn;
    }

    //public delegate void HitResponseCallback(HitResponse response);
    //public delegate void HitExceptionCallback(string message);
    


    /// <summary>
    /// 魚場
    /// </summary>
    public interface IFishStage
    {
        /// <summary>
        /// 玩家id
        /// </summary>
        long AccountId { get; }
        /// <summary>
        /// 魚場id
        /// </summary>
        byte FishStage { get; }
        
        /// <summary>
        /// 判定請求
        /// </summary>
        /// <param name="request"></param>
        void Hit(HitRequest request);

        /// <summary>
        /// 判定結果事件
        /// </summary>
        event Action<HitResponse> HitResponseEvent;
        /// <summary>
        /// 例外
        /// </summary>
        event Action<string> HitExceptionEvent;

        /// <summary>
        /// 返回魚場請求
        /// </summary>
        void Quit();

    }
}
