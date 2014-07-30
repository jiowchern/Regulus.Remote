using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class ActorDoWakin : Regulus.Utility.IUpdatable
    {
        private Serializable.EntityPropertyInfomation _Property;
        Regulus.Utility.TimeCounter Timer;

        public ActorDoWakin(Serializable.EntityPropertyInfomation property)
        {
            Timer = new Utility.TimeCounter();
            this._Property = property;
        }

        bool Utility.IUpdatable.Update()
        {
            return Timer.Second < 10;
        }

        void Framework.ILaunched.Launch()
        {
            
        }

        void Framework.ILaunched.Shutdown()
        {
            _Property.Energy = _Property.MaxEnergy;
            _Property.Died = false;            
        }
    }
}
