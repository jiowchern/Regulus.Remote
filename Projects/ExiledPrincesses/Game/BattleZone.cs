using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    public class Zone : IZone , Regulus.Game.IFramework
    {
        Regulus.Game.FrameworkRoot _Fields;
        
        public Zone()
        {
            _Fields = new Regulus.Game.FrameworkRoot();
        }
        Remoting.Value<IBattleAdmissionTickets> IZone.Open(BattleRequester requester)
        {
            Remoting.Value<IBattleAdmissionTickets> ret = new Remoting.Value<IBattleAdmissionTickets>();

            var field = new Field(requester.Battlers.ToArray());

            field.EndEvent += () =>
            {
                _Fields.RemoveFramework(field);
            };

            field.FirstEvent += (val) =>
            {
                ret.SetValue(val);
            };
            _Fields.AddFramework(field);

            return ret;
        }


        void Regulus.Game.IFramework.Launch()
        {
            
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            
        }

        bool Regulus.Game.IFramework.Update()
        {
            _Fields.Update();
            return true;
        }
    }
}
