using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
namespace FormulaUserBot
{
    class HitHandler : Regulus.Utility.IUpdatable
    {
        
        public static double Interval = 1.0f / 20.0;

        private VGame.Project.FishHunter.IFishStage _Stage;
        private VGame.Project.FishHunter.HitRequest _Request;
        bool _Enable;
        Regulus.Utility.TimeCounter _TimeCounter;

        public HitHandler(VGame.Project.FishHunter.IFishStage _Stage, VGame.Project.FishHunter.HitRequest request)
        {            
            this._Stage = _Stage;
            this._Request = request;
            _Enable = true;
            _TimeCounter = new Regulus.Utility.TimeCounter();
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            return _Enable;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Stage.HitResponseEvent += _Response;
            _Stage.Hit(_Request);
            _TimeCounter.Reset();
        }

        private void _Response(VGame.Project.FishHunter.HitResponse obj)
        {
            if (obj.FishID == _Request.FishID && obj.WepID == _Request.WepID )
            {
                _Enable = false;
                                
                Log.Instance.WriteLine(string.Format("時間{2}:請求{0}\n回應{1}", _Request.ShowMembers(), obj.ShowMembers(), _TimeCounter.Second));                

                

            }            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Stage.HitResponseEvent -= _Response;
        }
    }
}
