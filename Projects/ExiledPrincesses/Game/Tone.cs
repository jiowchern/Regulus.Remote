using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    public class ToneResource : Regulus.Utility.Singleton<ToneResource>
    {
        Dictionary<string, TonePrototype> _TonePrototypes;
        public ToneResource()
        {
            _TonePrototypes = new Dictionary<string, TonePrototype>();
            _TonePrototypes.Add("Credits", new TonePrototype() { Maps = new string[] { "Teaching" } });
        }
        internal TonePrototype Find(string name)
        {
            TonePrototype tone;
            if (_TonePrototypes.TryGetValue(name, out tone))
            {
                return tone;
            }
            return null;
        }
    }

    class Town : Regulus.Game.IStage , ITown
    {

        public delegate void OnToMap(string name);
        public event OnToMap ToMapEvent;
        TonePrototype _Prototype;
        Regulus.Remoting.ISoulBinder _Binder;
        public Town(TonePrototype prototype , Regulus.Remoting.ISoulBinder binder)
        {
            _Binder = binder;
            _Prototype = prototype;
            
        }
        void Regulus.Game.IStage.Enter()
        {
            _Binder.Bind<ITown>(this);
        }

        void _GotoMap(string destination)
        {
            var result = (from map in _Prototype.Maps where map == destination select map).FirstOrDefault();
            if (result != null)
            {
                ToMapEvent(destination);
            }
        }
        void Regulus.Game.IStage.Leave()
        {
            _Binder.Unbind<ITown>(this);
        }

        void Regulus.Game.IStage.Update()
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
    }
}
