using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Ghost
{
	public interface IGhost
	{
		void	OnEvent	(string name_event , object[] args);
		Guid	GetID	();
	}
}
