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

        internal void OnSelect(Regulus.Framework.GameModeSelector<VGame.Project.FishHunter.IUser> selector)
        {

            selector.AddFactoty("standalong", new VGame.Project.FishHunter.StandalongUserFactory(core));
            var provider = selector.CreateUserProvider("standalong");
            provider.Spawn("1");
            provider.Select("1");
        }
    }
}
