// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreThreadRequestHandler.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CoreThreadRequestHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public class CoreThreadRequestHandler : IUpdatable
	{
		private readonly IRequestQueue _Requester;

		private bool _Enable;

		public CoreThreadRequestHandler(IRequestQueue requester)
		{
			this._Requester = requester;
		}

		bool IUpdatable.Update()
		{
			this._Requester.Update();
			return this._Enable;
		}

		void IBootable.Launch()
		{
			this._Enable = true;
			this._Requester.BreakEvent += this._End;
		}

		void IBootable.Shutdown()
		{
			this._Requester.BreakEvent -= this._End;
		}

		private void _End()
		{
			this._Enable = false;
		}
	}
}