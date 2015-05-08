using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class PlayStage : IPlayer , Regulus.Utility.IStage
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private IFishStage _FishStage;
        int _Money;

        

        public delegate void DoneCallback(int money);
        public event DoneCallback DoneEvent;

        Dictionary<int, HitRequest> Requests;

        public PlayStage(Regulus.Remoting.ISoulBinder binder, IFishStage fish_stage , int money)
        {
            Requests = new Dictionary<int, HitRequest>();
            this._Binder = binder;
            this._FishStage = fish_stage;
            
            _Money = money;
        }

        private void _Response(HitResponse obj)
        {
            VGame.Project.FishHunter.HitRequest request ;
            if(Requests.TryGetValue(obj.FishID , out request ))
            {
                _DeathFishEvent(obj.FishID);

                AddMoney(request.WepBet * request.WepOdds);
                
            }
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<IPlayer>(this);
            _FishStage.HitResponseEvent += _Response;
        }

        void Regulus.Utility.IStage.Leave()
        {
            _FishStage.HitResponseEvent -= _Response;
            _Binder.Unbind<IPlayer>(this);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }

        void IPlayer.Hit(int fishid, short bullet_score)
        {
            if (bullet_score > 0 && HasMoney(bullet_score)  )
                return;

            AddMoney(-bullet_score);

            VGame.Project.FishHunter.HitRequest request = new VGame.Project.FishHunter.HitRequest();
            request.FishID = (short)fishid;
            request.FishOdds = 1;

            request.FishStatus = VGame.Project.FishHunter.FISH_STATUS.NORMAL;
            request.FishType = 1;
            request.TotalHits = 1;
            request.HitCnt = 1;
            request.TotalHitOdds = 1;
            request.WepBet = bullet_score;
            request.WepID = 1;
            request.WepOdds = 2;
            request.WepType = 1;
            _FishStage.Hit(request);
        }

        private void AddMoney(int p)
        {
            _Money += p;
            _MoneyEvent(_Money);
        }

        private bool HasMoney(short bullet_score)
        {
            return _Money >= bullet_score;
        }


        void IPlayer.Quit()
        {
            DoneEvent(_Money);
        }


        event Action<int> _DeathFishEvent;
        event Action<int> IPlayer.DeathFishEvent
        {
            add { _DeathFishEvent += value; }
            remove { _DeathFishEvent -= value; }
        }

        event Action<int> _MoneyEvent;
        event Action<int> IPlayer.MoneyEvent
        {
            add { _MoneyEvent += value; }
            remove { _MoneyEvent -= value; }
        }
    }
}
