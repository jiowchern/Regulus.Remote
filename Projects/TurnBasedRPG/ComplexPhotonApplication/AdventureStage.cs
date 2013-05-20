using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class AdventureStage : Samebest.Game.IStage<User>
    {
        DateTime _Save;
        Regulus.Project.TurnBasedRPG.Player _Player;
        void Samebest.Game.IStage<User>.Enter(User obj)
        {
            _Player = _BuildPlayer(obj.Actor);
            _Player.ExitWorldEvent += obj.ToParking;
            _Player.LogoutEvent += obj.Logout;
            obj.Provider.Bind<Common.IPlayer>(_Player);
            _Save = DateTime.Now;
        }

        private Player _BuildPlayer(Serializable.DBActorInfomation dB_actorInfomation)
        {
            return new Player(dB_actorInfomation);
        }

        void Samebest.Game.IStage<User>.Leave(User obj)
        {
            obj.Provider.Unbind<Common.IPlayer>(_Player);
        }

        void Samebest.Game.IStage<User>.Update(User obj)
        {
            var elapsed = DateTime.Now.Ticks - _Save.Ticks;
            var span = new TimeSpan(elapsed);
            if (span.TotalMinutes > 1.0)
            {
                Samebest.Utility.Singleton<Storage>.Instance.SaveActor(obj.Actor);
                _Save = DateTime.Now;
            }
        }
    }
}
