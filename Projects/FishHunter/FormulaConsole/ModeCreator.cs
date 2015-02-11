using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    public class ModeCreator
    {
        private Regulus.Utility.ICore core;

        public ModeCreator(Regulus.Utility.ICore core)
        {
            // TODO: Complete member initialization
            this.core = core;
        }

        internal void OnSelect(Regulus.Framework.GameModeSelector<VGame.Project.FishHunter.Formula.IUser> selector)
        {

            selector.AddFactoty("standalong", new VGame.Project.FishHunter.Formula.StandalongUserFactory(core));
            selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.RemotingUserFactory());
            //var provider = selector.CreateUserProvider("standalong");
            //var provider = selector.CreateUserProvider("remoting");
            
            
            //provider.Spawn("1");
            //provider.Select("1");
        }
    }
}
