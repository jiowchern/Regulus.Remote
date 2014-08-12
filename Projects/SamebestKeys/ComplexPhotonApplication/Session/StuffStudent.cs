using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Session
{
    class StuffStudent : 
        Regulus.Project.SamebestKeys.ISessionStudent , 
        Regulus.Project.SamebestKeys.ISessionStuff ,
        Regulus.Project.SamebestKeys.ISessionRequester

    {
        public delegate void OnSpeak(float[] values);
        public event OnSpeak SpeakEvent;
        RequestLession _ResuestLession;

        internal void NoTeacher()
        {
            _Binder.Bind<ISessionRequester>(this);
            _Requester.SetValue(false);
            _Requester = null;
        }

        internal void TeacherBusy()
        {
            _Binder.Bind<ISessionRequester>(this);
            
            //_Requester.SetValue(false);
            _Requester = null;
        }

        internal void NoCoin()
        {
            _Binder.Bind<ISessionRequester>(this);
            _Requester.SetValue(false);
            _Requester = null;
        }

        internal RequestLession PopRequester()
        {
            var temp = _ResuestLession;
            if (_ResuestLession != null)
                _ResuestLession = null;
            return temp;
        }

        internal bool CheckCoin(string lession)
        {
            var coin = (from l in Lession.Lessions where l.Name == lession select l).SingleOrDefault();
            return _CheckCoin(coin);
        }

        private bool _CheckCoin(Lession lession)
        {
            return true;
        }

        public string Name { get { return _Player.Name; } }

        internal void Done(Score score)
        {            
            _Binder.Bind<ISessionRequester>(this);
            if (_DoneEvent != null)
                _DoneEvent(score.Scores);
        }

        internal void Next(int step)
        {
            if (_NextEvent != null)
                _NextEvent(step);
        }

        internal void Texture(string texture)
        {
            if (_TextureEvent != null)
                _TextureEvent(texture);
        }

        internal void TeacherSpeak(float[] values)
        {
            _TeacherSpeakEvent(values);
        }
        string _Lession;
        internal void BeginLession(string lession)
        {
            _Lession = lession;
            _Binder.Unbind<ISessionRequester>(this);
            _Binder.Bind<ISessionStuff>(this);
            _Binder.Bind<ISessionStudent>(this);
        }

        event Action<long> ISessionStudent.CurrentTimeEvent
        {
            add {  }
            remove {  }
        }

        event Action<int> _NextEvent;
        event Action<int> ISessionStudent.NextEvent
        {
            add { _NextEvent += value; }
            remove { _NextEvent -= value; }
        }


        event Action<string> _TextureEvent;
        event Action<string> ISessionStudent.TextureEvent
        {
            add { _TextureEvent+= value; }
            remove { _TextureEvent -= value; }
        }

        Guid ISessionStuff.Id
        {
            get { return _Player.Id; }
        }

        event Action<float[]> _TeacherSpeakEvent;
        event Action<float[]> ISessionStuff.SpeakEvent
        {
            add { _TeacherSpeakEvent+= value; }
            remove { _TeacherSpeakEvent-= value; }
        }

        void ISessionStuff.Speak(float[] speak)
        {
            SpeakEvent(speak);
        }

        string ISessionStudent.Lession
        {
            get { return _Lession; }
        }



        event Action<SessionScore[]> _DoneEvent;
        event Action<SessionScore[]> ISessionStudent.DoneEvent
        {
            add { _DoneEvent += value; }
            remove { _DoneEvent -= value; }
        }


        Remoting.Value<bool> _Requester;
        private Player _Player;

        public StuffStudent(Player player)
        {            
            this._Player = player;
        }
        Remoting.Value<bool> ISessionRequester.Requester(string target, int coin)
        {
            var lession = (from l in Lession.Lessions where l.Coin <= coin orderby l.Coin descending select l).FirstOrDefault();
            if (_Requester != null)
                _Requester.SetValue(false);

            _Requester = new Remoting.Value<bool>();
            _Requester.OnValue += _UnbindSessionRequester;
            _ResuestLession = new RequestLession() { Name = target, Lession = lession.Name, Answer = _Requester };
            return _Requester;            
        }

        private void _UnbindSessionRequester(bool obj)
        {
            _Requester = null;            
        }

        Remoting.ISoulBinder _Binder;
        private bool _Enable;
        internal void SetBinder(Remoting.ISoulBinder binder)
        {
            _Enable = true;
            _Binder = binder;

            _Binder.Bind<ISessionRequester>(this);
            _Binder.BreakEvent += _Break;
        }

        private void _Break()
        {
            _Enable = false;
        }
        public bool Enable { get { return _Enable; } }

        internal void EndLession()
        {
            _Binder.Unbind<ISessionStuff>(this);
            _Binder.Unbind<ISessionStudent>(this);
            _Binder.Bind<ISessionRequester>(this);
        }

        internal void ClearBinder()
        {
            _Binder.Unbind<ISessionStuff>(this);
            _Binder.Unbind<ISessionStudent>(this);
            _Binder.Unbind<ISessionRequester>(this);
        }
    }
}
