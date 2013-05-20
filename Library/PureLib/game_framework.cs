using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Game
{
	public interface IFramework
	{
		void Launch();
		bool Update();
		void Shutdown();
	}
}
