using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.PhotonExpansion
{
	public interface IPhotonFramework : Regulus.Utility.IUpdatable
	{

		void ObtainController(Soul.SoulProvider provider);
	}


	public abstract class PhotonFramework : Regulus.Utility.Updater<Regulus.Utility.IUpdatable>, IPhotonFramework
    {
        
        public abstract void ObtainController(Soul.SoulProvider provider);
        void IPhotonFramework.ObtainController(Soul.SoulProvider provider)
        {
            ObtainController(provider);
        }

        public abstract void Launch();


		void Regulus.Framework.ILaunched.Launch()
        {            
            Launch();
        }

		bool Regulus.Utility.IUpdatable.Update()
        {
            Update();
            return true;
        }

        public abstract void Shutdown();
		void Regulus.Framework.ILaunched.Shutdown()
        {
            Shutdown();
        }
    }
}
