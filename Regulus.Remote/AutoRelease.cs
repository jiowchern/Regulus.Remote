using Regulus.Serialization;
using System;
using System.Collections.Generic;



namespace Regulus.Remote
{
    public class AutoRelease
    {
        private readonly Dictionary<long, WeakReference<IGhost>> _Exists;

        private readonly IGhostRequest _Requester;
        private readonly ISerializer _Serializer;

        public AutoRelease(IGhostRequest _Requester, ISerializer serializer)
        {
            this._Requester = _Requester;
            _Serializer = serializer;
            _Exists = new Dictionary<long, WeakReference<IGhost>>();
        }


        public void Register(IGhost ghost)
        {
            long id = ghost.GetID();
            WeakReference<IGhost> instance;
            if (_Exists.TryGetValue(id, out instance) == false)
            {
                _Exists.Add(id, new WeakReference<IGhost>(ghost));
            }
        }


        public void Update()
        {
            List<long> ids = new List<long>();

            foreach (KeyValuePair<long, WeakReference<IGhost>> e in _Exists)
            {
                IGhost target = null;


                if (!e.Value.TryGetTarget(out target))
                {
                    ids.Add(e.Key);
                }
            }

            foreach (long id in ids)
            {

                _Exists.Remove(id);


                if (_Requester != null)
                {
                    PackageRelease data = new PackageRelease();
                    data.EntityId = id;
                    _Requester.Request(ClientToServerOpCode.Release, data.ToBuffer(_Serializer));
                }
            }
        }
    }
}
