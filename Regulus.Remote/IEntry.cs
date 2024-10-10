using System.Diagnostics;

namespace Regulus.Remote
{    
    public interface IEntry : IBinderProvider
    {
        void Update();
    }
}
