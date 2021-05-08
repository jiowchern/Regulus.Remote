using System;

namespace Regulus.Remote
{

    public interface IBinder
    {
        event Action BreakEvent;

        ISoul Return<TSoul>(TSoul soul);

        ISoul Bind<TSoul>(TSoul soul);

        void Unbind(ISoul soul);
    }
}
