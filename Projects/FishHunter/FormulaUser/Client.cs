// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Client type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Utility;

#endregion

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