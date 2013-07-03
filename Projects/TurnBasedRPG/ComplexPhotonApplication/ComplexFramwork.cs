using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{

    class LocalTime : Regulus.Utility.Singleton<Regulus.Remoting.Time>, Regulus.Game.IFramework
    {

        void Regulus.Game.IFramework.Launch()
        {
                
        }

        bool Regulus.Game.IFramework.Update()
        {
            Instance.Update();
            return true;    
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            
        }
    }

    class ComplexFramwork : Regulus.Remoting.PhotonExpansion.PhotonFramework 
    {        
        //Regulus.Project.TurnBasedRPG.Map    _Map;
        Regulus.Project.TurnBasedRPG.Hall   _Hall;


        LocalTime _Time;
        public ComplexFramwork()
        {
            _Time = new LocalTime();
            _Hall = new Hall();
        }

        public override void ObtainController(Regulus.Remoting.Soul.SoulProvider provider)
        {
            _Hall.PushUser(new User(provider , _Hall));
        }

        public override void Launch()
        {
            Regulus.Utility.Singleton<Map>.Instance.SetTime(LocalTime.Instance);
            AddFramework(_Time);            
            AddFramework(Regulus.Utility.Singleton<Map>.Instance);            
            AddFramework(_Hall);  
            AddFramework(Regulus.Utility.Singleton<Storage>.Instance);                        
        }

        public override void Shutdown()
        {
            RemoveFramework(Regulus.Utility.Singleton<Storage>.Instance);            
            RemoveFramework(_Hall);
            RemoveFramework(Regulus.Utility.Singleton<Map>.Instance);
            RemoveFramework(_Time);            
        }
    }
}
