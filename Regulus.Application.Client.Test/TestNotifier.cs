using Regulus.Remote;
using System;

namespace Regulus.Application.Client.Test
{
    internal class TestNotifier<T> : INotifier<T>
    {
        T[] INotifier<T>.Ghosts => throw new NotImplementedException();

        T[] INotifier<T>.Returns => throw new NotImplementedException();

        event Action<T> INotifier<T>.Return
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event Action<T> _Supply;
        event Action<T> INotifier<T>.Supply
        {
            add
            {
                _Supply += value;
            }

            remove
            {
                _Supply -= value;
            }
        }

        event Action<T> INotifier<T>.Unsupply
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }
}