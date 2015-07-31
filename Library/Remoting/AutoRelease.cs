// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoRelease.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AutoRelease type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

#endregion

namespace Regulus.Remoting
{
	public class AutoRelease
	{
		private readonly Dictionary<Guid, WeakReference> _Exists;

		private readonly IGhostRequest _Requester;

		public AutoRelease(IGhostRequest _Requester)
		{
			this._Requester = _Requester;
			_Exists = new Dictionary<Guid, WeakReference>();
		}

		public void Register(IGhost ghost)
		{
			var id = ghost.GetID();
			WeakReference instance;
			if (_Exists.TryGetValue(id, out instance) == false)
			{
				_Exists.Add(id, new WeakReference(ghost));
			}
		}

		public void Update()
		{
			var ids = new List<Guid>();

			foreach (var e in _Exists)
			{
				if (e.Value.IsAlive == false)
				{
					ids.Add(e.Key);
				}
			}

			foreach (var id in ids)
			{
				var args = new Dictionary<byte, byte[]>();
				args[0] = id.ToByteArray();

				_Exists.Remove(id);
				if (_Requester != null)
				{
					_Requester.Request((int)ClientToServerOpCode.Release, args);
				}
			}
		}
	}
}