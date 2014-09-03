using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility.Coroutine
{
    public interface IUpdateable : Regulus.Framework.ILaunched
    {
        IEnumerator<INext> Enumerator();
    }


    public interface INext 
    {

        bool IsDone();
    }


    
    public class WaitFrame : INext 
    {
        int _CheckCount;
        public WaitFrame(int frames)
        {
            _CheckCount = frames;
            System.Diagnostics.Debug.Assert(_CheckCount > 0);
        }
        bool INext.IsDone()
        {            
            return --_CheckCount == 0;
        }
    }

    public class WaitOneFrame : WaitFrame
    {
        public WaitOneFrame() :base(1){}
    }

    public class WaitSeconds : INext
    {

        float _Second;
        Regulus.Utility.TimeCounter _TimeCounter;
        public WaitSeconds(float second)
        {
            _Second = second;
            _TimeCounter = new TimeCounter();
        }

        bool INext.IsDone()
        {
            return _TimeCounter.Second >= _Second;
        }
    }


    public class Coroutine 
    {

        class Unit : Regulus.Framework.ILaunched
        {
            public IUpdateable Updateable;
            public IEnumerator<INext> Enumerator;

            void Framework.ILaunched.Launch()
            {
                Updateable.Launch();
            }

            void Framework.ILaunched.Shutdown()
            {
                Updateable.Shutdown();
            }
        }

        Launcher<Unit> _Units;
        public Coroutine()
        {
            _Units = new Launcher<Unit>();
        }
        public void Add(IUpdateable framework)
        {
            _Units.Add(new Unit() { Enumerator = framework.Enumerator(), Updateable = framework });
        }

        public void Remove(IUpdateable framework)
        {
            var units = from u in _Units.Objects where u.Updateable == framework select u;
            foreach (var unit in units)
                _Units.Remove(unit);
        }
        
        public void Update()
        {
            foreach (var cur in _Units.Update())
            {
                var updater = cur.Updateable;

                var enumerator = cur.Enumerator;

                if (enumerator == null)
                {
                    Remove(updater);
                    continue;
                }                    

                var next = enumerator.Current;

                if (next == null)
                {
                    Remove(updater);
                    continue;
                }

                if (next.IsDone())
                {
                    if (enumerator.MoveNext() == false)
                    {
                        Remove(updater);
                        continue;
                    }    
                }

            } 

        }

        public void Shutdown()
        {
            _Units.Shutdown();            
        }
    }
}
