using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Standalong
{
	public class Provider : Regulus.Remoting.ISoulBinder
	{
		
		public Provider()
		{
			
		}
		void Remoting.ISoulBinder.Bind<T>(T soul)
		{
		
		}

		void Remoting.ISoulBinder.Unbind<T>(T soul)
		{
				
		}

		event Action Remoting.ISoulBinder.BreakEvent
		{
			add {  }
			remove {  }
		}

		public void Register<T>(Regulus.Remoting.Ghost.IProviderNotice<T> provider)
		{
		
		}

		public void Unregister<T>()
		{
		
		}
	}
}
