using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Stage
{
    public class Formula : Regulus.Utility.IStage , VGame.Project.FishHunter.IFishStage
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private long _Account;
        private int _Stage;
        private FishHunter.Formula.HitBase _Formula;

        public delegate void DoneCallback();
        public event DoneCallback DoneEvent;


        public Formula(Regulus.Remoting.ISoulBinder binder, long account, int stage_id, FishHunter.Formula.HitBase formula)
        {            
            this._Binder = binder;
            this._Account = account;
            this._Stage = stage_id;
            this._Formula = formula;
        }
        

        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<VGame.Project.FishHunter.IFishStage>(this);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<VGame.Project.FishHunter.IFishStage>(this);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }

        long IFishStage.AccountId
        {
            get { return _Account; }
        }

        byte IFishStage.FishStage
        {
            get { return (byte)_Stage; }
        }

        void IFishStage.Hit(HitRequest request)
        {
            var response = _Formula.Request(request);
            
            _HitResponseEvent(response);
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
            add { _HitExceptionEvent += value ; }
            remove { _HitExceptionEvent-= value; }
        }


        void IFishStage.Quit()
        {
            DoneEvent();
        }
    }
}
