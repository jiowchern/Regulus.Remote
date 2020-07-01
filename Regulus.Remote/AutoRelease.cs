using System;
using System.Collections.Generic;
using Regulus.Serialization;
namespace Regulus.Remote
{
	public class AutoRelease
	{
		private readonly Dictionary<long, WeakReference<IGhost>> _Exists;

		private readonly IGhostRequest _Requester;
	    private readonly ISerializer _Serializer;

	    public AutoRelease(IGhostRequest _Requester , ISerializer serializer)
		{
			this._Requester = _Requester;
	        _Serializer = serializer;
	        _Exists = new Dictionary<long, WeakReference<IGhost>>();
		}


        public void Register(IGhost ghost)
		{
			var id = ghost.GetID();
			WeakReference<IGhost> instance;
			if(_Exists.TryGetValue(id, out instance) == false)
			{
				_Exists.Add(id, new WeakReference<IGhost>(ghost));
			}
		}


        public void Update()
		{
			var ids = new List<long>();

			foreach(var e in _Exists)
			{
                IGhost target = null;
                

                if (!e.Value.TryGetTarget(out target))
				{
					ids.Add(e.Key);
				}
			}

			foreach(var id in ids)
			{				

				_Exists.Remove(id);


				if(_Requester != null)
				{
                    var data = new PackageRelease();
				    data.EntityId = id;
                    _Requester.Request(ClientToServerOpCode.Release, data.ToBuffer(_Serializer));
				}
			}
		}
	}
}
