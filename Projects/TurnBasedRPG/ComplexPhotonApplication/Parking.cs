using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Parking : Common.IParking
    {
        Guid _Id;
        List<Serializable.DBActorInfomation> _ActorInfomations = new List<Serializable.DBActorInfomation>();
        public Parking(Guid id)
        {
            _Id = id;
            _ActorInfomations = Samebest.Utility.Singleton<Storage>.Instance.FindActor(_Id).ToList();
        }
        Samebest.Remoting.Value<bool> Common.IParking.CheckActorName(string name)
        {
            return Samebest.Utility.Singleton<Storage>.Instance.CheckActorName(name);
        }

        Samebest.Remoting.Value<bool> Common.IParking.CreateActor(Serializable.ActorInfomation cai)
        {
            if (Samebest.Utility.Singleton<Storage>.Instance.CheckActorName(cai.Name) == false)
            {
                Serializable.DBActorInfomation ai = new Serializable.DBActorInfomation();
                ai.Name = cai.Name;
                ai.Owner = _Id;
                _ActorInfomations.Add(ai);
                Samebest.Utility.Singleton<Storage>.Instance.Add(ai);
                return true;
            }
            return false;
        }
        
        Samebest.Remoting.Value<Serializable.ActorInfomation[]> Common.IParking.QueryActors()
        {
            return _ActorInfomations.ToArray();
        }


        

        public event Action<Serializable.DBActorInfomation> SelectEvent;
        void Common.IParking.Select(string name)
        {
            var a = _ActorInfomations.Find( (ai)=>{ return ai.Name == name ; });
            if (a != null && SelectEvent != null)
            {
                SelectEvent(a);
                SelectEvent = null;
            }
        }


        Samebest.Remoting.Value<Serializable.ActorInfomation[]> Common.IParking.DestroyActor(string name)
        {
            var res = Samebest.Utility.Singleton<Storage>.Instance.RemoveActor(_Id, name);
            if (res)
            {
                _ActorInfomations.Remove((from ai in _ActorInfomations where ai.Name == name select ai).FirstOrDefault());
            }
            return _ActorInfomations.ToArray();
        }


        public event Action BackEvent;
        void Common.IParking.Back()
        {
            if (BackEvent != null)
            {
                BackEvent();
                BackEvent = null;
            }
        }
    }
}
