// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verify.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Verify type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Datas;
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace VGame.Project.FishHunter
{
	public class Verify : IVerify
	{
		public event DoneCallback OnDoneEvent;

		private readonly IAccountFinder _Storage;

		public Verify(IAccountFinder storage)
		{
			_Storage = storage;
		}

		Value<bool> IVerify.Login(string id, string password)
		{
			var returnValue = new Value<bool>();
			var val = _Storage.FindAccountByName(id);
			val.OnValue += account =>
			{
				var found = account != null;
				if (found && account.IsPassword(password))
				{
					if (OnDoneEvent != null)
					{
						OnDoneEvent(account);
					}

					returnValue.SetValue(true);
				}
				else
				{
					returnValue.SetValue(false);
				}
			};
			return returnValue;
		}

		public delegate void DoneCallback(Account account);
	}
}