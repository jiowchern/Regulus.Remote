using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Standalong
{
	public class GhostProvider : Regulus.Remoting.ISoulBinder
	{
		
		void Remoting.ISoulBinder.Bind<TSoul>(TSoul soul)
		{
							
		}

		void Remoting.ISoulBinder.Unbind<TSoul>(TSoul soul)
		{
			
		}
	}
}
