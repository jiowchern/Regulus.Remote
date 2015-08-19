using System;
using System.Collections.Generic;
using System.Linq;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;


using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.Stage
{
    internal class QuarterStage : IFishStage
    {
        private event Action<HitResponse[]> _OnTotalHitResponseEvent;

        private readonly int _FishStage;

        public QuarterStage(Guid player_id, int fish_stage)
        {
            AccountId = player_id;
            _FishStage = fish_stage;
        }

        event Action<string> IFishStage.OnHitExceptionEvent
        {
            add { }
            remove { }
        }

        event Action<HitResponse[]> IFishStage.OnTotalHitResponseEvent
        {
            add { _OnTotalHitResponseEvent += value; }
            remove { _OnTotalHitResponseEvent -= value; }
        }

        public Guid AccountId { get; }

        int IFishStage.FishStage
        {
            get { return _FishStage; }
        }

        void IFishStage.Hit(HitRequest request)
        {
            _OnTotalHitResponseEvent(
                request.FishDatas.Select(
                    requset_fish_data => new HitResponse
                    {
                        DieResult =
                            Random.Instance.NextInt(1, 4) == 1 ? FISH_DETERMINATION.DEATH : FISH_DETERMINATION.SURVIVAL, 
                        FishId = requset_fish_data.FishId, 
                        WepId = request.WeaponData.WepId, 
                        FeedbackWeaponType = new[]
                        {
                            WEAPON_TYPE.INVALID
                        }
                    }).ToArray());
        }
    }
}
