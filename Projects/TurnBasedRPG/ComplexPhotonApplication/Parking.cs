using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Parking : IParking
    {
        Guid _Id;
        List<Serializable.DBEntityInfomation> _ActorInfomations = new List<Serializable.DBEntityInfomation>();
        public Parking(Guid id)
        {
            _Id = id;
            _ActorInfomations = Regulus.Utility.Singleton<Storage>.Instance.FindActor(_Id).ToList();
        }
        Regulus.Remoting.Value<bool> IParking.CheckActorName(string name)
        {
            return Regulus.Utility.Singleton<Storage>.Instance.CheckActorName(name);
        }

        Regulus.Remoting.Value<bool> IParking.CreateActor(Serializable.EntityLookInfomation cai)
        {
            if (Regulus.Utility.Singleton<Storage>.Instance.CheckActorName(cai.Name) == false)
            {
                Serializable.DBEntityInfomation ai = new Serializable.DBEntityInfomation();
                ai.Look.Name = cai.Name;				                
                
				var position = ai.Property.Position;
                ai.Property.Id = Guid.NewGuid();				
				
                ai.Owner = _Id;
                _ActorInfomations.Add(ai);
                Regulus.Utility.Singleton<Storage>.Instance.Add(ai);
                return true;
            }
            return false;
        }
        
        Regulus.Remoting.Value<Serializable.EntityLookInfomation[]> IParking.QueryActors()
        {
            return (from actor in _ActorInfomations select actor.Look).ToArray();            
        }

        public event Action<Serializable.DBEntityInfomation> SelectEvent;
        Regulus.Remoting.Value<bool> IParking.Select(string name)
        {
            var a = _ActorInfomations.Find( (ai)=>{ return ai.Look.Name == name ; });
            if (a != null && SelectEvent != null)
            {
                SelectEvent(a);
                SelectEvent = null;
                return true;
            }
            return false;
        }


        Regulus.Remoting.Value<Serializable.EntityLookInfomation[]> IParking.DestroyActor(string name)
        {
            var res = Regulus.Utility.Singleton<Storage>.Instance.RemoveActor(_Id, name);
            if (res)
            {
                _ActorInfomations.Remove((from ai in _ActorInfomations where ai.Look.Name == name select ai).FirstOrDefault());
            }
            return (from actor in _ActorInfomations select actor.Look).ToArray();
        }


        public event Action BackEvent;
        void IParking.Back()
        {
            if (BackEvent != null)
            {
                BackEvent();
                BackEvent = null;
            }
        }
    }
}
