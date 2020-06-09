using System;
using System.Reflection;

namespace Regulus.Remote
{
    public class PropertyUpdater
    {
        private readonly IDirtyable _Dirtyable;
        public readonly int PropertyId;
        


        bool _Dirty;
        bool _Close;
        object _Object;
        public object Value => _Object;

        public PropertyUpdater(IDirtyable dirtyable, int id)
        {
            this._Dirtyable = dirtyable;
            this.PropertyId = id;            

            _Dirtyable.DirtyEvent += _SetDirty;
        }

        private void _SetDirty(object arg2)
        {
            _Dirty = true;
            _Object = arg2;
        }


        public bool Update()
        {
            if (_Close)
                return false;
            if(_Dirty)
            {
                _Dirty = false;
                _Close = true;
                return true;
            }
            return false;
        }
        public void Release()
        {
            _Dirtyable.DirtyEvent -= _SetDirty;
        }

        internal void Reset()
        {
            _Close = false;
        }
    }
}