using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    public class ModeCreator
    {
        private Regulus.Remoting.ICore core;

        public ModeCreator(Regulus.Remoting.ICore core)
        {
            // TODO: Complete member initialization
            this.core = core;
        }

        internal void OnSelect(Regulus.Framework.GameModeSelector<VGame.Project.FishHunter.IUser> selector)
        {

            selector.AddFactoty("standalong", new VGame.Project.FishHunter.StandalongUserFactory(core));
            selector.AddFactoty("remoting", new VGame.Project.FishHunter.RemotingUserFactory());
            //var provider = selector.CreateUserProvider("standalong");
            var provider = selector.CreateUserProvider("remoting");
            
            
            provider.Spawn("1");
            provider.Select("1");
        }
    }
}
