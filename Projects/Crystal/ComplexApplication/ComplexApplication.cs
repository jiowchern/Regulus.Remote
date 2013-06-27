using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Project.Crystal
{
	public class ComplexApplication : Samebest.Remoting.Soul.PhotonApplication
    {
		protected override Samebest.Remoting.PhotonExpansion.IPhotonFramework _Setup()
		{
			return new Regulus.Project.Crystal.ComplexFramework();
		}
	}
}
