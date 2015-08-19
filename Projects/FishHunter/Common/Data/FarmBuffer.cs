using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
    /// <summary>
    ///     The data.
    /// </summary>
    /// [ProtoContract]
    [ProtoContract]
    public class FarmBuffer
    {
        public enum BUFFER_BLOCK
        {
            BLOCK_1, 

            BLOCK_2, 

            BLOCK_3, 

            BLOCK_4, 

            BLOCK_5, 
        }

        public enum BUFFER_TYPE
        {
            NORMAL, 

            SPEC, 

            VIR00, 

            VIR01, 

            VIR02, 

            VIR03, 
        }

        [ProtoContract]
        public class BufferValue
        {
            [ProtoMember(1)]
            public Guid Id { get; set; }

            [ProtoMember(2)]
            public int FireCount { get; set; }

            [ProtoMember(3)]
            public int AverageValue { get; set; }

            [ProtoMember(4)]
            public int AverageTimes { get; set; }

            [ProtoMember(5)]
            public int AverageTotal { get; set; }

            [ProtoMember(6)]
            public int HiLoRate { get; set; }

            [ProtoMember(7)]
            public int RealTime { get; set; }

            public BufferValue()
            {
                Id = new Guid();
            }
        }

        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public BUFFER_BLOCK BufferBlock { get; set; }

        [ProtoMember(3)]
        public BUFFER_TYPE BufferType { get; set; }

        [ProtoMember(4)]
        public int Buffer { get; set; }

        [ProtoMember(5)]
        public int Rate { get; set; }

        [ProtoMember(6)]
        public int Count { get; set; }

        [ProtoMember(7)]
        public int Top { get; set; }

        [ProtoMember(8)]
        public int Gate { get; set; }

        [ProtoMember(9)]
        public BufferValue BufferTempValue { get; private set; }

        public FarmBuffer()
        {
            Id = new Guid();
            BufferTempValue = new BufferValue();
        }
    }
}
