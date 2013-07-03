using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Project.Crystal
{
	public class ComplexApplication : Regulus.Remoting.Soul.PhotonApplication
    {
		protected override Regulus.Remoting.PhotonExpansion.IPhotonFramework _Setup()
		{
			return new Regulus.Project.Crystal.ComplexFramework();
		}
	}
}
