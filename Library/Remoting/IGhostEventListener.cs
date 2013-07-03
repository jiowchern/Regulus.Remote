using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost
{
	public interface IGhost
	{
		void	OnEvent	(string name_event , object[] args);
		Guid	GetID	();
        void    OnProperty(string name, object value);
    }
}
