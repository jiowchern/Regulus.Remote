using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Player : Entity , Common.IPlayer 
    {
        private Serializable.DBEntityInfomation _DBactorInfomation;

        public Player(Serializable.DBEntityInfomation dB_actorInfomation)
            : base(dB_actorInfomation.Property)
        {            
            this._DBactorInfomation = dB_actorInfomation;
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
            _DBactorInfomation.TestData = ++i;
            return _DBactorInfomation.TestData;
        }


        Samebest.Remoting.Value<int> Common.IPlayer.GetData()
        {
            return _DBactorInfomation.TestData;
        }
    }
}
