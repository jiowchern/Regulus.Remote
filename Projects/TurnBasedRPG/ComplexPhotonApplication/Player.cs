using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Player : Actor , Common.IPlayer  
    {
        private Serializable.DBEntityInfomation _DBActorInfomation;

        public Player(Serializable.DBEntityInfomation dB_actorInfomation)
            : base(dB_actorInfomation)
        {            
            _DBActorInfomation = dB_actorInfomation;
        }

        public event Action LogoutEvent;
        void Common.IPlayer.Logout()
        {
            if (LogoutEvent != null)
            {
                LogoutEvent();
            }
        }

        public event Action ExitWorldEvent;
        void Common.IPlayer.ExitWorld()
        {
            if (ExitWorldEvent != null)
            {
                ExitWorldEvent();
            }
        }

        Samebest.Remoting.Value<int> Common.IPlayer.SetData(int i)
        {
            _DBActorInfomation.TestData = ++i;
            return _DBActorInfomation.TestData;
        }

        Samebest.Remoting.Value<int> Common.IPlayer.GetData()
        {
            return _DBActorInfomation.TestData;
        }
    }
}
