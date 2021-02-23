using System;

namespace Regulus.Remote
{
    class NotifierUpdater   : System.IDisposable
    {
        private readonly int _Id;
        private readonly ITypeObjectNotifiable _Notifiable;

        public event System.Action<int, TypeObject> SupplyEvent;
        public event System.Action<int, TypeObject> UnsupplyEvent;
        public NotifierUpdater(int id , ITypeObjectNotifiable notifiable)
        {
            this._Id = id;
            this._Notifiable = notifiable;
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
            UnsupplyEvent(_Id , obj);
        }

        private void _Supply(TypeObject obj)
        {
            SupplyEvent(_Id, obj);
        }
    }
}
