// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Root type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

#endregion

namespace Regulus.Framework
{
	public class Root<TUser>
	{
		private readonly IUserFactoty<TUser> _Provider;

		private readonly List<TUser> _Users;

		public Root(IUserFactoty<TUser> provider)
		{
			this._Provider = provider;
			this._Users = new List<TUser>();
		}

		public TUser Spawn()
		{
			var user = this._Provider.SpawnUser();
			this._Users.Add(user);
			return user;
		}

		public void Unspawn(TUser user)
		{
			this._Users.Remove(user);
		}
	}
}