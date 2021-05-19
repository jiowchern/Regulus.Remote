using System;
using System.Linq;


namespace Regulus.Remote
{
    class NotifierUpdater   
    {
        
        readonly int _Id;
        readonly ITypeObjectNotifiable _Notifiable;
        readonly System.Collections.Generic.List<ISoul> _Souls;
        public event System.Func<int, TypeObject,ISoul> SupplyEvent;
        public event System.Action<int, long> UnsupplyEvent;
        public NotifierUpdater(int id , ITypeObjectNotifiable notifiable)
        {            
            this._Id = id;
            this._Notifiable = notifiable;
            _Souls = new System.Collections.Generic.List<ISoul>();
        }
        
    
        internal void Initial()
        {
            _Notifiable.SupplyEvent += _Create;
            _Notifiable.UnsupplyEvent += _Destroy;
        }

        internal void Finial()
        {
            _Notifiable.SupplyEvent -= _Create;
            _Notifiable.UnsupplyEvent -= _Destroy;

            ISoul[] souls;
            lock(_Souls)
            {
                souls = _Souls.ToArray();
                _Souls.Clear();
            }
            

            foreach (var soul in souls)
            {
                UnsupplyEvent(_Id, soul.Id);
            }

        }

        private void _Destroy(TypeObject obj)
        {
            ISoul soul;
            lock(_Souls)
            {
                soul = (from s in _Souls where s.IsTypeObject(obj) select s).First();
                _Souls.Remove(soul);
            }            
            UnsupplyEvent(_Id , soul.Id);
        }

        private void _Create(TypeObject obj)
        {
            var soul = SupplyEvent(_Id,obj);
            lock(_Souls)
                _Souls.Add( soul);
        }

        
    }
}
