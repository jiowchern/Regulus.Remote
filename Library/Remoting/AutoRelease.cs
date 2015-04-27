using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public class AutoRelease
    {
        private Regulus.Remoting.IGhostRequest _Requester;
        Dictionary<Guid, WeakReference>_Exists;
        public AutoRelease(Regulus.Remoting.IGhostRequest _Requester)
        {
            // TODO: Complete member initialization
            this._Requester = _Requester;
            _Exists = new Dictionary<Guid, WeakReference>();
        }
        public void Register(Regulus.Remoting.Ghost.IGhost ghost)
        {
            var id = ghost.GetID();
            WeakReference instance ;
            if (_Exists.TryGetValue(id, out instance) == false)
                _Exists.Add(id, new WeakReference(ghost));
        }

        public void Update()
        {
            List<Guid> ids = new List<Guid>();

            foreach(var e in _Exists)
            {
                if(e.Value.IsAlive == false)
                {
                    ids.Add(e.Key);
                }
            }

            foreach(var id in ids)
            {
                var args = new Dictionary<byte, byte[]>();
                args[0] = id.ToByteArray();

                _Exists.Remove(id);
                _Requester.Request((int)ClientToServerOpCode.Release, args);
            }
        }
    }
}
