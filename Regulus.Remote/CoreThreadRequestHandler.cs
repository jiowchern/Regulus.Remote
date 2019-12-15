using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Remote
{
	public class CoreThreadRequestHandler : IUpdatable
	{
		private readonly IRequestQueue _Requester;

		private bool _Enable;

		public CoreThreadRequestHandler(IRequestQueue requester)
		{
			_Requester = requester;
		}

		bool IUpdatable.Update()
		{
			_Requester.Update();
			return _Enable;
		}

		void IBootable.Launch()
		{
			_Enable = true;
			_Requester.BreakEvent += _End;
		}

		void IBootable.Shutdown()
		{
			_Requester.BreakEvent -= _End;
		}

		private void _End()
		{
			_Enable = false;
		}
	}
}
