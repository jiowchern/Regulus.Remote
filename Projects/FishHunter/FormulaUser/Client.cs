using Regulus.Framework;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Formula
{
	public class Client : Client<IUser>
	{
		public Client()
			: base(new DummyView(), new DummyInput())
		{
		}

		public Client(Console.IViewer view, Console.IInput input)
			: base(view, input)
		{
		}
	}
}
