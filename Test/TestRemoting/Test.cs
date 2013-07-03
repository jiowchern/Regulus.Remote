using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestRemotingCommon
{
	public interface ITest
	{

        string Value { get; }
		event Action	a1 ;
		event Action<int> a2;

		Regulus.Remoting.Value<int> TestMethod(int a23);
	}
	
}
