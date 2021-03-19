using System;

namespace Regulus.Remote
{

    public interface IBinder
    {
        event Action BreakEvent;

        IProxy Return<TSoul>(TSoul soul);

        IProxy Bind<TSoul>(TSoul soul);

        void Unbind(IProxy soul);
    }
}
