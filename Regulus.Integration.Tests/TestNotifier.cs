using Regulus.Remote;
using System;

namespace Regulus.Integration.Tests
{
    internal class TestNotifier<T> : INotifier<T>
    {




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