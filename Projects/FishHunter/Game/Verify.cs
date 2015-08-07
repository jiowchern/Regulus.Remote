#region Test_Region

using Regulus.Remoting;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

#endregion

namespace VGame.Project.FishHunter
{
	public class Verify : IVerify
	{

		public delegate void DoneCallback(Account account);

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
	}
}