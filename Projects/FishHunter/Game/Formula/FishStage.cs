using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
namespace VGame.Project.FishHunter.Formula
{
    class FishStage : IFishStage
    {
        private FishHunter.Formula.HitBase _Formula;

        byte _FishStage;
        long _AccountId;
        public FishStage(long account, int stage_id)
        {
            _Formula = new FishHunter.Formula.HitTest(Regulus.Utility.Random.Instance.R);
            _AccountId = account;
            _FishStage =(byte) stage_id;
        }
        long IFishStage.AccountId
        {
            get { return _AccountId; }
        }

        byte IFishStage.FishStage
        {
            get { return _FishStage; }
        }

        void IFishStage.Hit(HitRequest request)
        {
            var response = _Formula.Request(request);
            _HitResponseEvent(response);


            Regulus.Utility.Log.Instance.Write(response.ShowMembers());
        }
        event Action<HitResponse> _HitResponseEvent;
        event Action<HitResponse> IFishStage.HitResponseEvent
        {
            add
            {
                _HitResponseEvent += value;
            }
            remove { _HitResponseEvent -= value; }
        }

        event Action<string> _HitExceptionEvent;
        event Action<string> IFishStage.HitExceptionEvent
        {
            add { _HitExceptionEvent += value; }
            remove { _HitExceptionEvent -= value; }
        }

        
    }
}
