using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProtobuf
{
    [ProtoBuf.ProtoContract]    
    class Data
    {
        [ProtoBuf.ProtoMember(1)]
        public string var1 { get;set;}
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1, typeof(Regulus.Utility.Map<byte, byte[]>))]
    public class Package
    {
        [ProtoBuf.ProtoMember(1)]
        public byte Code;
        [ProtoBuf.ProtoMember(2)]
        public Regulus.Utility.Map<byte, byte[]> Args;
    }

    [ProtoBuf.ProtoContract]
    [Serializable]
    public enum ActionStatue
    {
        Idle,
        Greeting,
        Bow,
        Talk,
        Run,
        Happy,
        Sad,
        GangnamStyle
    }

    class Program
    {
        static void Main(string[] args)
        {
            Package package = new Package(); ;
            package.Code = 123;
            package.Args = new Regulus.Utility.Map<byte, byte[]>();
            
            var streamData = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize<Data>(streamData, new Data() { var1 = "1234567890" });
            streamData.Position = 0;
            var p2 = ProtoBuf.Serializer.Deserialize<Data>(streamData);
            package.Args.Add(100, streamData.ToArray());

            System.IO.MemoryStream streamActionStatue = new System.IO.MemoryStream();
            ActionStatue actionStatus = new ActionStatue();
            actionStatus = ActionStatue.GangnamStyle;
            ProtoBuf.Serializer.Serialize<ActionStatue>(streamActionStatue, actionStatus);
            streamActionStatue.Position = 0;
            var p1 = ProtoBuf.Serializer.Deserialize<ActionStatue>(streamActionStatue);
            package.Args.Add(120, streamActionStatue.ToArray());


            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize<Package>(stream, package);
            stream.Position = 0;
            var p = ProtoBuf.Serializer.Deserialize<Package>(stream);
        }
    }
}
