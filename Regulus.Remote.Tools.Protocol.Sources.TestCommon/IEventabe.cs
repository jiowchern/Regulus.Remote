namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public delegate void CustomDelegate();
    public interface IEventabe : IEventabe2
    {
        event System.Action<int, string, float, double, decimal, System.Guid> Event01;

        event System.Action Event02;

        event CustomDelegate CustomDelegateEvent;
    }
}
