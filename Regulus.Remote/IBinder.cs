using System;

namespace Regulus.Remote
{

    public interface IBinder
    {
        event Action BreakEvent;

        void Return<TSoul>(TSoul soul);

        void Bind<TSoul>(TSoul soul);

        void Unbind<TSoul>(TSoul soul);
    }
}
