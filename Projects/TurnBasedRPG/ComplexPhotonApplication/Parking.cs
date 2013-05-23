using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Parking : Common.IParking
    {
        Guid _Id;
        List<Serializable.DBEntityInfomation> _ActorInfomations = new List<Serializable.DBEntityInfomation>();
        public Parking(Guid id)
        {
            _Id = id;
            _ActorInfomations = Samebest.Utility.Singleton<Storage>.Instance.FindActor(_Id).ToList();
        }
        Samebest.Remoting.Value<bool> Common.IParking.CheckActorName(string name)
        {
            return Samebest.Utility.Singleton<Storage>.Instance.CheckActorName(name);
        }

        Samebest.Remoting.Value<bool> Common.IParking.CreateActor(Serializable.EntityLookInfomation cai)
        {
            if (Samebest.Utility.Singleton<Storage>.Instance.CheckActorName(cai.Name) == false)
            {
                Serializable.DBEntityInfomation ai = new Serializable.DBEntityInfomation();
                ai.Look.Name = cai.Name;                
                ai.Owner = _Id;
                _ActorInfomations.Add(ai);
                Samebest.Utility.Singleton<Storage>.Instance.Add(ai);
                return true;
            }
            return false;
        }
        
        Samebest.Remoting.Value<Serializable.EntityLookInfomation[]> Common.IParking.QueryActors()
        {
            return (from actor in _ActorInfomations select actor.Look).ToArray();            
        }

        public event Action<Serializable.DBEntityInfomation> SelectEvent;
        void Common.IParking.Select(string name)
        {
            var a = _ActorInfomations.Find( (ai)=>{ return ai.Look.Name == name ; });
            if (a != null && SelectEvent != null)
            {
                SelectEvent(a);
                SelectEvent = null;
            }
        }


        Samebest.Remoting.Value<Serializable.EntityLookInfomation[]> Common.IParking.DestroyActor(string name)
        {
            var res = Samebest.Utility.Singleton<Storage>.Instance.RemoveActor(_Id, name);
            if (res)
            {
                _ActorInfomations.Remove((from ai in _ActorInfomations where ai.Look.Name == name select ai).FirstOrDefault());
            }
            return (from actor in _ActorInfomations select actor.Look).ToArray();
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
