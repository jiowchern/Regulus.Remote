using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestRemotingCommon
{
	public interface ITest
	{
		event Action	a1 ;
		event Action<int> a2;

		Samebest.Remoting.Value<int> TestMethod(int a23);
	}
	
}
