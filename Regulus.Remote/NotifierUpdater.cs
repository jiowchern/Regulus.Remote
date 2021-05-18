using System;
using System.Linq;


namespace Regulus.Remote
{
    class NotifierUpdater   : System.IDisposable
    {
        readonly System.Collections.Generic.List<ISoul>_Souls;
        readonly System.Collections.Generic.List<TypeObject> _Instances;
        private readonly int _Id;
        private readonly ITypeObjectNotifiable _Notifiable;

        public delegate ISoul OnSupply(int protocol_id, TypeObject obj);
        public delegate void OnUnsupply(int protocol_id, long soul_id);
        event OnSupply _SupplyEvent;
        public event OnSupply SupplyEvent {
            add
            {
                foreach (var instance in _Instances)
                {
                    var soul = value(_Id, instance);
                    lock(_Souls)
                        _Souls.Add(soul);
                }
                _SupplyEvent += value;
            }
            remove
            {
                _SupplyEvent -= value;
            }
        }
        event OnUnsupply _UnsupplyEvent;
        public event OnUnsupply UnsupplyEvent
        {
            
            add{ _UnsupplyEvent += value; }
            remove { _UnsupplyEvent -= value; }
        }
        public NotifierUpdater(int id , ITypeObjectNotifiable notifiable)
        {
            this._Id = id;
            this._Notifiable = notifiable;
            _Souls = new System.Collections.Generic.List<ISoul>();
            _Instances = new System.Collections.Generic.List<TypeObject>();
            _Notifiable.SupplyEvent += _Supply;
            _Notifiable.UnsupplyEvent += _Unsupply;            
        }
        
        void IDisposable.Dispose()
        {
            _Notifiable.SupplyEvent -= _Supply;
            _Notifiable.UnsupplyEvent -= _Unsupply;
        }

        private void _Unsupply(TypeObject obj)
        {
            lock(_Instances)
                _Instances.Remove(obj);


            var souls = from s in _Souls
                        where s.IsTypeObject(obj)
                        select s;
            var soul = souls.First();
            lock(_Souls)
                _Souls.Remove(soul);

            _UnsupplyEvent(_Id, soul.Id);
        }

        private void _Supply(TypeObject obj)
        {
            lock(_Instances)
                _Instances.Add(obj);
            if (_SupplyEvent == null)
                return;
            var soul = _SupplyEvent(_Id, obj);
            lock(_Souls)
                _Souls.Add(soul);
        }
    }
}
