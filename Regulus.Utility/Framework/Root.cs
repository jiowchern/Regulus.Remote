using System.Collections.Generic;

namespace Regulus.Framework
{
	public class Root<TUser>
	{
		private readonly IUserFactoty<TUser> _Provider;

		private readonly List<TUser> _Users;

		public Root(IUserFactoty<TUser> provider)
		{
			_Provider = provider;
			_Users = new List<TUser>();
		}

		public TUser Spawn()
		{
			var user = _Provider.SpawnUser();
			_Users.Add(user);
			return user;
		}

		public void Unspawn(TUser user)
		{
			_Users.Remove(user);
		}
	}
}
