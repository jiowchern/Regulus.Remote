using System;
using System.Collections.Generic;

namespace Regulus.Remote
{
    public class GhostNotifier<T> : INotifier<T>
    {
        private readonly Action<PassageCallback> _AddSupplyHandler;
        private readonly Action<PassageCallback> _RemoveSupplyHandler;
        private readonly Action<PassageCallback> _AddUnsupplyHandler;
        private readonly Action<PassageCallback> _RemoveUnsupplyHandler;
        readonly List<T> _Gpis;
        readonly List<GhostPassage<T>> _SupplyPassages;
        readonly List<GhostPassage<T>> _UnsupplyPassages;
        public GhostNotifier(Action<PassageCallback> add_supply_handler, Action<PassageCallback> remove_supply_handler,
            Action<PassageCallback> add_unsupply_handler, Action<PassageCallback> remove_unsupply_handler
            )
        {
            _Gpis = new List<T>();
            _AddSupplyHandler = add_supply_handler;
            _RemoveSupplyHandler = remove_supply_handler;
            _AddUnsupplyHandler = add_unsupply_handler;
            _RemoveUnsupplyHandler = remove_unsupply_handler;

            _SupplyPassages = new List<GhostPassage<T>>();
            _UnsupplyPassages = new List<GhostPassage<T>>();
        }

        T[] INotifier<T>.Ghosts => _Gpis.ToArray();
       

        event Action<T> INotifier<T>.Supply
        {
            add
            {
                foreach (var gpi in _Gpis)
                {
                    value(gpi);
                }
                _AddSupplyHandler.Invoke(_Add(value));
            }

            remove
            {
                var passage = _SupplyPassages.Find(p => p.Owner == value);
                if (passage != null)
                    _RemoveSupplyHandler.Invoke(passage.Through);
            }
        }

        private PassageCallback _Add(Action<T> value)
        {
            var passage = new GhostPassage<T>(value);
            passage.ThroughEvent += _Gpis.Add;
            _SupplyPassages.Add(passage);
            return passage.Through;
            
        }

        private PassageCallback _Remove(Action<T> value)
        {

            var passage = new GhostPassage<T>(value);
            passage.ThroughEvent += (gpi) => _Gpis.Remove(gpi);
            _UnsupplyPassages.Add(passage);
            return passage.Through;
        }

      

        event Action<T> INotifier<T>.Unsupply
        {
            add
            {
                _AddUnsupplyHandler.Invoke(_Remove(value));
            }

            remove
            {
                var passage = _UnsupplyPassages.Find(p => p.Owner == value);
                if(passage != null)
                    _RemoveUnsupplyHandler.Invoke(passage.Through);
            }
        }

        

        
    }
}