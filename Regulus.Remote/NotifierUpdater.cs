using System;
using System.Linq;


namespace Regulus.Remote
{
    class NotifierUpdater   : System.IDisposable
    {
        readonly System.Collections.Generic.List<ISoul>_Souls;
        private readonly int _Id;
        private readonly ITypeObjectNotifiable _Notifiable;

        public delegate ISoul OnSupply(int protocol_id, TypeObject obj);
        public delegate void OnUnsupply(int protocol_id, long soul_id);
        public event OnSupply SupplyEvent;
        public event OnUnsupply UnsupplyEvent;
        public NotifierUpdater(int id , ITypeObjectNotifiable notifiable)
        {
            this._Id = id;
            this._Notifiable = notifiable;
            _Notifiable.SupplyEvent += _Supply;
            _Notifiable.UnsupplyEvent += _Unsupply;
            _Souls = new System.Collections.Generic.List<ISoul>();
        }
        
        void IDisposable.Dispose()
        {
            _Notifiable.SupplyEvent -= _Supply;
            _Notifiable.UnsupplyEvent -= _Unsupply;
        }

        private void _Unsupply(TypeObject obj)
        {
            var souls = from s in _Souls
                        where s.IsTypeObject(obj)
                        select s;
            var soul = souls.First();
            _Souls.Remove(soul);
            UnsupplyEvent(_Id, soul.Id);
        }

        private void _Supply(TypeObject obj)
        {
            var soul = SupplyEvent(_Id, obj);

            _Souls.Add(soul);
        }
    }
}
