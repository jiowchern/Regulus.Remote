using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class BotPlayStage 
        : Regulus.Utility.IStage
    {
        VGame.Project.FishHunter.IFishStage _Stage;
        public delegate void DoneCallback();
        public event DoneCallback DoneEvent;

        Regulus.Utility.TimeCounter _HitTime;
        Regulus.Utility.Updater _HitHandlers;

        public BotPlayStage(VGame.Project.FishHunter.IFishStage fish_stage)
        {
            _HitHandlers = new Regulus.Utility.Updater();
            _HitTime = new Regulus.Utility.TimeCounter();
            _Stage = fish_stage;
            
        }
        void Regulus.Utility.IStage.Leave()
        {
            _HitHandlers.Shutdown();
        }
        void Regulus.Utility.IStage.Enter()
        {
            
        }

        void Regulus.Utility.IStage.Update()
        {
            _HitHandlers.Update();
            if(_HitTime.Second > 1.0f / 10.0f)
            {
                //var totalHits = (byte)Regulus.Utility.Random.Next(1, 2);
                var totalHits = (byte)1;
                for (int i = 0; i < totalHits; ++i)
                    _HitRequest(totalHits);
                _HitTime.Reset();
            }
        }

        private void _HitRequest(byte total_hits)
        {
            VGame.Project.FishHunter.HitRequest request = new VGame.Project.FishHunter.HitRequest();
            request.FishID = (short)Regulus.Utility.Random.Next(0,32767);
            request.FishOdds = (short)Regulus.Utility.Random.Next(1, 1000);
            
            request.FishStatus = Regulus.Utility.Random.NextEnum<VGame.Project.FishHunter.FISH_STATUS>();
            request.FishType = (byte)Regulus.Utility.Random.Next(1, 99);
            request.TotalHits = total_hits;
            request.HitCnt = (short)Regulus.Utility.Random.Next(1, request.TotalHits);
            request.TotalHitOdds = (short)Regulus.Utility.Random.Next(0, 32767); ;
            request.WepBet = (short)Regulus.Utility.Random.Next(1, 10000);
            request.WepID = (short)Regulus.Utility.Random.Next(0, 32767);
            request.WepOdds = (short)Regulus.Utility.Random.Next(1, 10000);
            request.WepType = 1;


            var hitHandler = new HitHandler(_Stage, request);
            _HitHandlers.Add(hitHandler);

            
        }
    }
}
