// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Launcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

using Regulus.Framework;

#endregion

namespace Regulus.Utility
{
	public class Launcher
	{
		private readonly List<IBootable> _Launchers;

		public int Count
		{
			get { return this._Launchers.Count; }
		}

		public Launcher()
		{
			this._Launchers = new List<IBootable>();
		}

		public void Push(IBootable laucnher)
		{
			this._Launchers.Add(laucnher);
		}

		public void Shutdown()
		{
			foreach (var l in this._Launchers)
			{
				l.Shutdown();
			}
		}

		public void Launch()
		{
			foreach (var l in this._Launchers)
			{
				l.Launch();
			}
		}
	}
}