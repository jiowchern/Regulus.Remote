using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    public class ComplexApplication : Samebest.Remoting.Soul.PhotonApplication
    {
        protected override Samebest.Remoting.PhotonExpansion.IPhotonFramework _Setup()
        {            
            return new Regulus.Project.TurnBasedRPG.ComplexFramwork();
        }
    }
}
