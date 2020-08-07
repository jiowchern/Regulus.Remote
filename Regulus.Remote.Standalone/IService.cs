using System;

namespace Regulus.Remote.Standalone
{
    public interface IService : IDisposable
    {
        INotifierQueryable CreateNotifierQueryer();
        void DestroyNotifierQueryer(INotifierQueryable queryable);

        void Update();
    }
}
