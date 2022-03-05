namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public interface IEventabe : IEventabe2
    {
        event System.Action<int, string, float, double, decimal, System.Guid> Event01;
    }
}
