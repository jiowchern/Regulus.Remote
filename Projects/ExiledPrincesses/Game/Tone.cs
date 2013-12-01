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

    class Tone : Regulus.Game.IStage
    {

        public delegate void OnToMap(string name);
        public event OnToMap ToMapEvent;
        TonePrototype _Prototype;

        public Tone(TonePrototype prototype)
        {
            _Prototype = prototype;
        }
        void Regulus.Game.IStage.Enter()
        {
            
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
            
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
