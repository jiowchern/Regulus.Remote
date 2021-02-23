using System;

namespace Regulus.Remote
{
    public class PropertyUpdater : IPropertyIdValue
    {
        private readonly IDirtyable _Dirtyable;
        public readonly int PropertyId;
        public event System.Action<object> ChnageEvent;
        bool _Dirty;
        bool _Close;
        object _Object;
        object _StatusObject;
        

        int IPropertyIdValue.Id => PropertyId;

        object IPropertyIdValue.Instance => _StatusObject;

        public PropertyUpdater(IDirtyable dirtyable, int id)
        {
            this._Dirtyable = dirtyable;
            this.PropertyId = id;

            _Dirtyable.ChangeEvent += _SetDirty;
        }

        private void _SetDirty(object instance)
        {
            _Dirty = true;
            _Object = instance;

            if(_Update())
            {
                ChnageEvent(_Object);
            }
        }

       

        bool _Update()
        {            
          
            if (_Close)
                return false;
            if (_Dirty)
            {
                _Dirty = false;
                _Close = true;
                _StatusObject = _Object;
                return true;
            }
            return false;
        }

        
        public void Release()
        {
            _Dirtyable.ChangeEvent -= _SetDirty;
        }

        internal void Reset()
        {
            _Close = false;
            if(_Update())
            {
                ChnageEvent(_Object);
            }
        }
    }
}