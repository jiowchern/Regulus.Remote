using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{

    class LocalTime : Samebest.Utility.Singleton<Samebest.Remoting.Time>, Samebest.Game.IFramework
    {

        void Samebest.Game.IFramework.Launch()
        {
                
        }

        bool Samebest.Game.IFramework.Update()
        {
            Instance.Update();
            return true;    
        }

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }

    class ComplexFramwork : Samebest.Remoting.PhotonExpansion.PhotonFramework 
    {        
        //Regulus.Project.TurnBasedRPG.Map    _Map;
        Regulus.Project.TurnBasedRPG.Hall   _Hall;


        LocalTime _Time;
        public ComplexFramwork()
        {
            _Time = new LocalTime();
            _Hall = new Hall();
        }

        public override void ObtainController(Samebest.Remoting.Soul.SoulProvider provider)
        {
            _Hall.PushUser(new User(provider , _Hall));
        }

        public override void Launch()
        {
            Samebest.Utility.Singleton<Map>.Instance.SetTime(LocalTime.Instance);
            AddFramework(_Time);            
            AddFramework(Samebest.Utility.Singleton<Map>.Instance);            
            AddFramework(_Hall);  
            AddFramework(Samebest.Utility.Singleton<Storage>.Instance);                        
        }

        public override void Shutdown()
        {
            RemoveFramework(Samebest.Utility.Singleton<Storage>.Instance);            
            RemoveFramework(_Hall);
            RemoveFramework(Samebest.Utility.Singleton<Map>.Instance);
            RemoveFramework(_Time);            
        }
    }
}
