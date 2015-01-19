using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    public class TownResource : Regulus.Utility.Singleton<TownResource>
    {
        Dictionary<string, TownPrototype> _TonePrototypes;
        public TownResource()
        {
            _TonePrototypes = new Dictionary<string, TownPrototype>();
            _TonePrototypes.Add("Credits", new TownPrototype() { Maps = new string[] { "Teaching" } });
        }
        internal TownPrototype Find(string name)
        {
            TownPrototype tone;
            if (_TonePrototypes.TryGetValue(name, out tone))
            {
                return tone;
            }
            return null;
        }
    }

    class Town : Regulus.Utility.IStage , ITown
    {

        public delegate void OnToMap(string name);
        public event OnToMap ToLevelsEvent;

        TownPrototype _Prototype;
        Regulus.Remoting.ISoulBinder _Binder;
        string _Name;
        public Town(string name,TownPrototype prototype , Regulus.Remoting.ISoulBinder binder)
        {
            _Name = name;
            _Binder = binder;
            _Prototype = prototype;
            
        }
        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<ITown>(this);
        }

        void _GotoMap(string destination)
        {
            var result = (from map in _Prototype.Maps where map == destination select map).FirstOrDefault();
            if (result != null)
            {
                ToLevelsEvent(destination);                
            }
        }
        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<ITown>(this);
        }

        void Regulus.Utility.IStage.Update()
        {


        }

        string[] ITown.Maps
        {
            get { return _Prototype.Maps; }
        }


        void ITown.ToMap(string map)
        {
            _GotoMap(map);
        }


        string ITown.Name
        {
            get { return _Name; }
        }
    }
}
