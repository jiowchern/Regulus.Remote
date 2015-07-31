// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemotingClient.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DummyInputView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	internal class DummyInputView : Console.IInput, Console.IViewer
	{
		event Console.OnOutput Console.IInput.OutputEvent
		{
			add { }
			remove { }
		}

		void Console.IViewer.WriteLine(string message)
		{
		}

		void Console.IViewer.Write(string message)
		{
		}
	}

	public class RemotingClient : Client
	{
		public event UserCallback UserEvent;

		private RemotingClient(Console.IInput input, Console.IViewer view)
			: base(view, input)
		{
			ModeSelectorEvent += RemotingClient_ModeSelectorEvent;
		}

		public delegate void UserCallback(IUser user);

		private void RemotingClient_ModeSelectorEvent(GameModeSelector<IUser> selector)
		{
			selector.AddFactoty("remoting", new RemotingUserFactory());

			var provider = selector.CreateUserProvider("remoting");

			var user = provider.Spawn("1");
			provider.Select("1");
			if (UserEvent != null)
			{
				UserEvent(user);
			}
		}

		public static RemotingClient Create()
		{
			var dummpy = new DummyInputView();
			var client = new RemotingClient(dummpy, dummpy);
			return client;
		}
	}
}