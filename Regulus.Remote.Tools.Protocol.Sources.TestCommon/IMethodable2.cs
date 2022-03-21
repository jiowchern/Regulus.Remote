namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public class HelloRequest
    {
        public string Name;
    }

    public class HelloReply
    {
        public string Message;
    }
    

    public interface IMethodable2 : IMethodable1
    {
        Regulus.Remote.Value<int> GetValue2();
        Regulus.Remote.Value<HelloReply> SayHello(HelloRequest request);
    }
}
