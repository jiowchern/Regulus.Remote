using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.PhotonExpansion
{
	public interface IPhotonFramework : Regulus.Game.IFramework
	{

		void ObtainController(Soul.SoulProvider provider);
	}


    public abstract class PhotonFramework : Regulus.Game.FrameworkRoot , IPhotonFramework
    {
        
        public abstract void ObtainController(Soul.SoulProvider provider);
        void IPhotonFramework.ObtainController(Soul.SoulProvider provider)
        {
            ObtainController(provider);
        }

        public abstract void Launch();
        void Game.IFramework.Launch()
        {            
            Launch();
        }

        bool Game.IFramework.Update()
        {
            Update();
            return true;
        }

        public abstract void Shutdown();
        void Game.IFramework.Shutdown()
        {
            Shutdown();
        }
    }
}
