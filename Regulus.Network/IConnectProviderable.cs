namespace Regulus.Network
{
    public interface IConnectProviderable : Framework.IBootable
    {        
        IConnectable Spawn();
    }
}