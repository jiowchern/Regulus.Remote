

namespace Regulus.Remote.Unity
{
    public interface IAdsorptionAgent
    {
        string Name { get; }
        Regulus.Remote.Unity.Distributor Distributor
        {
            get;
        }

    }


}