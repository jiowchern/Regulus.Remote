using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    public class ComplexApplication : Regulus.Remoting.Soul.PhotonApplication
    {
        protected override Regulus.Remoting.PhotonExpansion.IPhotonFramework _Setup()
        {            
            return new Regulus.Project.TurnBasedRPG.ComplexFramwork();
        }
    }
}
