using Regulus.Utility;
using System;

namespace Regulus.Collection
{
    public interface IQuadObject
    {
        event EventHandler BoundsChanged;

        Rect Bounds { get; }
    }
}
