using System;
using System.Collections.Generic;

namespace Regulus.Remote
{
    internal class GhostNotifier<T> : INotifier<T>
    {
        private readonly Action<PassageCallback> _AddSupplyHandler;
        private readonly Action<PassageCallback> _RemoveSupplyHandler;
        private readonly Action<PassageCallback> _AddUnsupplyHandler;
        private readonly Action<PassageCallback> _RemoveUnsupplyHandler;
        readonly List<T> _Gpis;

        public GhostNotifier(Action<PassageCallback> add_supply_handler, Action<PassageCallback> remove_supply_handler,
            Action<PassageCallback> add_unsupply_handler, Action<PassageCallback> remove_unsupply_handler
            )
        {
            _Gpis = new List<T>();
            _AddSupplyHandler = add_supply_handler;
            _RemoveSupplyHandler = remove_supply_handler;
            _AddUnsupplyHandler = add_unsupply_handler;
            _RemoveUnsupplyHandler = remove_unsupply_handler;
        }

        T[] INotifier<T>.Ghosts => _Gpis.ToArray();

        T[] INotifier<T>.Returns => new T[0];

        event Action<T> INotifier<T>.Return
        {
            add
            {
             
            }

            remove
            {
             
            }
        }

        event Action<T> INotifier<T>.Supply
        {
            add
            {
                _AddSupply(_Add(value));
            }

            remove
            {
                _RemoveSupply((gpi) => value((T)gpi));
            }
        }

        private Action<T> _Add(Action<T> value)
        {
            return (gpi) => {
                _Gpis.Add(gpi);
                value(gpi);
            };
        }

        private Action<T> _Remove(Action<T> value)
        {
            return (gpi) => {
                _Gpis.Remove(gpi);
                value(gpi);
            };
        }

        private void _AddSupply(Action<T> value)
        {
            _AddSupplyHandler.Invoke((gpi)=> value((T)gpi));
        }
        private void _RemoveSupply(Action<T> value)
        {
            _RemoveSupplyHandler.Invoke((gpi) => value((T)gpi));
        }

        event Action<T> INotifier<T>.Unsupply
        {
            add
            {
                _AddUnsupply(_Remove(value));
            }

            remove
            {
                _RemoveUnsupply((gpi)=>value((T)gpi));
            }
        }

        private void _RemoveUnsupply(Action<object> value)
        {
            _RemoveUnsupplyHandler.Invoke((gpi) => value((T)gpi));

        }

        private void _AddUnsupply(Action<T> value)
        {
            _AddUnsupplyHandler.Invoke((gpi) => value((T)gpi));
        }
    }
}