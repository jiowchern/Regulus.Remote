namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public interface IEventabe1 : Regulus.Remote.Protocolable
    {
        event System.Action Event1;
        event System.Action<int> Event2;
        
    }
}
