namespace Regulus.Network
{
    public interface IConnectProvidable : Utiliey.IBootable
    {        
        IConnectable Spawn();
    }
}