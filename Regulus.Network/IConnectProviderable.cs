namespace Regulus.Network
{
    public interface IConnectProvidable : Utility.IBootable
    {        
        IConnectable Spawn();
    }
}