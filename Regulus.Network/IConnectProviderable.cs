namespace Regulus.Network
{
    public interface IConnectProvidable : Framework.IBootable
    {        
        IConnectable Spawn();
    }
}