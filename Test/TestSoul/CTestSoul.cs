using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSoul
{
	class CTestSoul : TestRemotingCommon.ITest
	{

		event Action a1;
		event Action<int> a2;
		event Action TestRemotingCommon.ITest.a1
		{
			add {  a1+= value ;}
			remove { a1-= value;  }
		}

		event Action<int> TestRemotingCommon.ITest.a2
		{
			add { a2+= value; }
			remove { a2-=value; }
		}


		public void invoke()
		{
			
		}


        Samebest.Remoting.Value<int> TestRemotingCommon.ITest.TestMethod(int a23)
		{
			a1.Invoke();
			a2.Invoke(a23);

            return new Samebest.Remoting.Value<int>(a23);
		}
	}
}
