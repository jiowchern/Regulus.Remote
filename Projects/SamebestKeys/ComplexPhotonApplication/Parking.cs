using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Parking : IParking
    {

        IStorage _Stroage;
        Guid _Id;
        List<Serializable.DBEntityInfomation> _ActorInfomations = new List<Serializable.DBEntityInfomation>();
        public Parking(Guid id, IStorage stroage)
        {
            _Stroage = stroage;
            _Id = id;
            _ActorInfomations = _Stroage.FindActor(_Id).ToList();
        }
        Regulus.Remoting.Value<bool> IParking.CheckActorName(string name)
        {
            return _Stroage.CheckActorName(name);
        }

        Regulus.Remoting.Value<bool> IParking.CreateActor(Serializable.EntityLookInfomation cai)
        {
            if (_Stroage.CheckActorName(cai.Name) == false)
            {
                Serializable.DBEntityInfomation ai = new Serializable.DBEntityInfomation();
                ai.Look.Name = cai.Name;				
				var position = ai.Property.Position;
                ai.Property.Id = Guid.NewGuid();
				position.X = (float)Regulus.Utility.Random.Instance.R.Next(0,100);
				position.Y = (float)Regulus.Utility.Random.Instance.R.Next(0,100);
				
                ai.Owner = _Id;
                _ActorInfomations.Add(ai);
                _Stroage.Add(ai);
                return true;
            }
            return false;
        }
        
        Regulus.Remoting.Value<Serializable.EntityLookInfomation[]> IParking.QueryActors()
        {
            return (from actor in _ActorInfomations select actor.Look).ToArray();            
        }

        public event Action<Serializable.DBEntityInfomation> SelectEvent;
        Regulus.Remoting.Value<string> IParking.Select(string name)
        {
            var a = _ActorInfomations.Find( (ai)=>{ return ai.Look.Name == name ; });
            if (a != null && SelectEvent != null)
            {                
                SelectEvent(a);
                SelectEvent = null;
                return a.Property.Map;
            }
            return string.Empty;
        }


        Regulus.Remoting.Value<Serializable.EntityLookInfomation[]> IParking.DestroyActor(string name)
        {
            var res = _Stroage.RemoveActor(_Id, name);
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
