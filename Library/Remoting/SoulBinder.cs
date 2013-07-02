using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
	public interface ISoulBinder
	{
		void Bind<TSoul>(TSoul soul);
		void Unbind<TSoul>(TSoul soul);
	}
}
