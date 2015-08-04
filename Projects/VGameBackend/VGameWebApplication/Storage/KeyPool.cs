// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyPool.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the KeyPool type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

using VGameWebApplication.Models;

#endregion

namespace VGameWebApplication.Storage
{
	internal class KeyPool : Singleton<KeyPool>
	{
		private readonly Dictionary<Guid, VerifyData> _Keys;

		public KeyPool()
		{
			this._Keys = new Dictionary<Guid, VerifyData>();
		}

		internal Guid Query(string user, string password)
		{
			var id = (from kp in this._Keys where kp.Value.Account == user select kp.Key).FirstOrDefault();
			if (id == Guid.Empty)
			{
				id = Guid.NewGuid();
				this._Keys.Add(id, new VerifyData
				{
					Account = user, 
					Password = password
				});
			}

			return id;
		}

		internal void Destroy(Guid guid)
		{
			this._Keys.Remove(guid);
		}

		internal VerifyData Find(Guid id)
		{
			VerifyData data = null;
			this._Keys.TryGetValue(id, out data);
			return data;
		}
	}
}