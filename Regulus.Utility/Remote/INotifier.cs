using System;

namespace Regulus.Remote
{
    /// <summary>
    ///     Remote object notifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INotifier<T>
    {

        /// <summary>
        ///     The server side object has been obtained.
        /// </summary>
        event Action<T> Supply;

        /// <summary>
        ///     The server side object has been closed.
        /// </summary>
        event Action<T> Unsupply;

        /// <summary>
        ///     An existing objects in a notifier.
        /// </summary>
        T[] Ghosts { get; }
    }
}
