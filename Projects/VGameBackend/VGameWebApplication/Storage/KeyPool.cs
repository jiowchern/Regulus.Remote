using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.Utility;


using VGameWebApplication.Models;

namespace VGameWebApplication.Storage
{
	internal class KeyPool : Singleton<KeyPool>
	{
		private readonly Dictionary<Guid, VerifyData> _Keys;

		public KeyPool()
		{
			_Keys = new Dictionary<Guid, VerifyData>();
		}

		internal Guid Query(string user, string password)
		{
			var id = (from kp in _Keys where kp.Value.Account == user select kp.Key).FirstOrDefault();
			if(id == Guid.Empty)
			{
				id = Guid.NewGuid();
				_Keys.Add(
					id, 
					new VerifyData
					{
						Account = user, 
						Password = password
					});
			}

			return id;
		}

		internal void Destroy(Guid guid)
		{
			_Keys.Remove(guid);
		}

		internal VerifyData Find(Guid id)
		{
			VerifyData data = null;
			_Keys.TryGetValue(id, out data);
			return data;
		}
	}
}
