using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
namespace VGame.Project.FishHunter.Play
{
    

    class PlayStage : IPlayer , Regulus.Utility.IStage
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private IFishStage _FishStage;
        Data.Record _Money;

        List<Bullet> _Bullets;
        List<Fish> _Fishs;
        
        public delegate void DoneCallback();
        public event DoneCallback DoneEvent;

        Dictionary<int, HitRequest> _Requests;

        public PlayStage(Regulus.Remoting.ISoulBinder binder, IFishStage fish_stage, Data.Record money)
        {
            _Fishs = new List<Fish>();
            _Bullets = new List<Bullet>();
            _Requests = new Dictionary<int, HitRequest>();
            this._Binder = binder;
            this._FishStage = fish_stage;
            
            _Money = money;
        }

        private void _Response(HitResponse obj)
        {
            VGame.Project.FishHunter.HitRequest request ;
            if(_Requests.TryGetValue(obj.FishID , out request ))
            {
                if (obj.DieResult == FISH_DETERMINATION.DEATH)
                {
                    _DeathFishEvent(obj.FishID);
                    AddMoney(request.WepBet * request.WepOdds);
                }
                else if(obj.DieResult == FISH_DETERMINATION.SURVIVAL)
                {
                    _PushFish(obj.FishID);
                }
                _Requests.Remove(obj.FishID);
            }
        }

        private void _PushFish(short id)
        {
            _Fishs.Add(new Fish(id));
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

        Regulus.Remoting.Value<int> IPlayer.Hit(int bulletid, int[] fishids)
        {            

            var hasBullet = _PopBullet(bulletid);
            if (hasBullet  == false)
                return 0;


            string logFishs ="";
            int count = 0;
            foreach(var fishid in fishids)
            {
                if (_PopFish(fishid) == false)
                    continue;

                if(_Requests.ContainsKey(fishid) == false)
                {
                    VGame.Project.FishHunter.HitRequest request = new VGame.Project.FishHunter.HitRequest();
                    request.FishID = (short)fishid;
                    request.FishOdds = 1;

                    request.FishStatus = VGame.Project.FishHunter.FISH_STATUS.NORMAL;
                    request.FishType = 1;
                    request.TotalHits = 1;
                    request.HitCnt = 1;
                    request.TotalHitOdds = 1;
                    request.WepBet = 1;
                    request.WepID = (short)bulletid;
                    request.WepOdds = 2;
                    request.WepType = 1;
                    _Requests.Add(fishid, request);
                    _FishStage.Hit(request);
                    count++;
                    logFishs += fishid.ToString() + ",";
                }
                
            }
            if (count == 0)
            {
                _PushBullet(bulletid);
            }

            Regulus.Utility.Log.Instance.Write(string.Format("all bullet:{0} , targets:{1} , count:{2}", bulletid, string.Join(",", (from id in fishids select id.ToString()).ToArray()), fishids.Length));
            Regulus.Utility.Log.Instance.Write(string.Format("requested bullet:{0} , targets:{1} , count:{2}", bulletid, logFishs, count));
            Regulus.Utility.Log.Instance.Write(string.Format("request fishs:{0} count:{1} ", string.Join(",", (from id in _Requests.Keys select id.ToString()).ToArray()), _Requests.Count));
            return count;
        }

        private bool _PopFish(int fishid)
        {
            return _Fishs.RemoveAll(fish => fish.Id == fishid) > 0;
        }

        private void _PushBullet(int bulletid)
        {
            _Bullets.Add(new Bullet(bulletid));
        }

        

        private bool _PopBullet(int bulletid)
        {            
            return _Bullets.RemoveAll(b => b.Id == bulletid) >  0;
        }

        private void AddMoney(int p)
        {
            _Money.Money += p;
            _MoneyEvent(_Money.Money);
        }

        private bool HasMoney(short bullet_score)
        {
            return _Money.Money >= bullet_score;
        }


        void IPlayer.Quit()
        {
            DoneEvent();
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

        Regulus.Remoting.Value<int> IPlayer.RequestBullet()
        {
            if (HasMoney(1) == false)
                return 0;

            AddMoney(-1);

            return _CreateBullet();
        }

        private Regulus.Remoting.Value<int> _CreateBullet()
        {
            var bullet = new Bullet();
            _Bullets.Add(bullet);
            return bullet.Id;
        }


        Regulus.Remoting.Value<short> IPlayer.RequestFish()
        {
            checked
            {
                var fishid = ++_FishIdSn;
                _Fishs.Add(new Fish(fishid));
                return fishid;
            }            
        }

        short _FishIdSn;
    }
}
