using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeys.Session
{
    class StuffTeacher : 
        Regulus.Project.SamebestKeys.ISessionTeacher, 
        Regulus.Project.SamebestKeys.ISessionResponse , 
        Regulus.Project.SamebestKeys.ISessionStuff
    {
        Score _Score;
        public delegate void OnDone(Score score);
        public event OnDone DoneEvent;

        public delegate void OnNext(int step);
        public event  OnNext NextEvent;

        public delegate void OnTexture(string texture);
        public event OnTexture TextureEvent;
        
        public event Action<float[]> StudentSpeakEvent;
        public string Name { get { return _Player.Name; } }


        Remoting.Value<bool> _Response;
        internal Remoting.Value<bool> RequestOpenRoom(Remoting.Value<bool> answer)
        {
            if (_Response != null)
            {
                _Binder.Unbind<ISessionResponse>(this);
                _Response.SetValue(false);
            }

            _Binder.Bind<ISessionResponse>(this);
            _Response = answer;
            return _Response;
        }
        

        internal void StudentSpeak(float[] values)
        {
            if (_SpeakEvent != null)
                _SpeakEvent(values);
        }


        string _Lession;
        internal void BeginLession(string lession)
        {
            _Lession = lession;
            _Binder.Bind<ISessionTeacher>(this);
            _Binder.Bind<ISessionStuff>(this);
        }

        event Action<long> ISessionTeacher.CurrentTimeEvent
        {
            add {  }
            remove {  }
        }

        void ISessionTeacher.Next(int step)
        {
            NextEvent(step);
        }

        void ISessionTeacher.SetScore(SessionScoreType type, int score)
        {
            //_Score[type] = score;
        }

        void ISessionTeacher.SetTexture(string name)
        {
            TextureEvent(name);
        }

        void ISessionTeacher.Done()
        {            
            DoneEvent(_Score);
        }

        string ISessionTeacher.Lession
        {
            get { return _Lession; }
        }

        void ISessionResponse.Yes()
        {            
            _Response.SetValue(true);
            _Response = null;
            _Binder.Unbind<ISessionResponse>(this);            
        }

        void ISessionResponse.No()
        {
            _Response.SetValue(false);
            _Response = null;
            _Binder.Unbind<ISessionResponse>(this);
        }

        Guid ISessionStuff.Id
        {
            get { return _Player.Id; }
        }

        event Action<float[]> _SpeakEvent;
        private Player _Player;

        public StuffTeacher(Player player)
        {
            // TODO: Complete member initialization
            this._Player = player;
            _Score = new Score();
        }
        event Action<float[]> ISessionStuff.SpeakEvent
        {
            add { _SpeakEvent += value; }
            remove { _SpeakEvent -= value; }
        }

        void ISessionStuff.Speak(float[] speak)
        {
            StudentSpeakEvent(speak);            
        }


        Remoting.ISoulBinder _Binder;
        private bool _Enable;
        internal void SetBinder(Remoting.ISoulBinder binder)
        {
            _Enable = true;
            _Binder = binder;

            _Binder.BreakEvent += _Break;
        }
        private void _Break()
        {
            _Enable = false;
        }
        public bool Enable { get { return _Enable; } }
        internal void EndLession()
        {
            _Binder.Unbind<ISessionTeacher>(this);
            _Binder.Unbind<ISessionStuff>(this);
        }

        internal void ClearBinder()
        {
            _Binder.Unbind<ISessionTeacher>(this);
            _Binder.Unbind<ISessionStuff>(this);
            _Binder.Unbind<ISessionResponse>(this);     
        }
    }
}
