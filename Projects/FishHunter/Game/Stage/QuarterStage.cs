using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Stage
{
    internal class QuarterStage : IFishStage
    {
        private readonly long _PlayerId;

        private readonly int _FishStage;

        public QuarterStage(long player_id, int fish_stage)
        {
            _PlayerId = player_id;
            _FishStage = fish_stage;
            
        }

        event System.Action<string> IFishStage.OnHitExceptionEvent
        {
            add {  }
            remove {  }
        }

        private event System.Action<Common.Data.HitResponse> _HitResponseEvent;
        event System.Action<Common.Data.HitResponse> IFishStage.OnHitResponseEvent
        {
            add { _HitResponseEvent += value; }
            remove { _HitResponseEvent -= value; }
        }

        long IFishStage.AccountId
        {
            get { return _PlayerId; }
        }

        int IFishStage.FishStage
        {
            get { return _FishStage; }
        }

        void IFishStage.Hit(Common.Data.HitRequest request)
        {
            foreach(var requsetFishData in request.FishDatas)
            {
                var response = new Common.Data.HitResponse();

                response.DieResult = Regulus.Utility.Random.Instance.NextInt(1,4) == 1 ? FISH_DETERMINATION.DEATH : FISH_DETERMINATION.SURVIVAL ;
                response.FishID = requsetFishData.FishID;
                response.WepID = request.WeaponData.WepID;            
                response.SpecialWeaponType = WEAPON_TYPE.NORMAL;            

                _HitResponseEvent(response);    
            }
            
        }
    }
}