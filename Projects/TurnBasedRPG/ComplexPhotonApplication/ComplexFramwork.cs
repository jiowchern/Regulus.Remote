using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{

    class LocalTime : Regulus.Utility.Singleton<Regulus.Remoting.Time>, Regulus.Utility.IUpdatable
    {

		void Regulus.Framework.ILaunched.Launch()
        {
                
        }

		bool Regulus.Utility.IUpdatable.Update()
        {
            Instance.Update();
            return true;    
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }

    class ComplexFramwork : Regulus.Remoting.PhotonExpansion.PhotonFramework 
    {        
        //Regulus.Project.TurnBasedRPG.Map    _Map;
        Regulus.Project.TurnBasedRPG.Hall   _Hall;

		World								_World;
        LocalTime _Time;
        public ComplexFramwork()
        {
            _Time = new LocalTime();
            _Hall = new Hall();
			_World = new World(LocalTime.Instance);
        }

        public override void ObtainController(Regulus.Remoting.Soul.SoulProvider provider)
        {
			_Hall.PushUser(new User(provider, _Hall, _World));
        }

        public override void Launch()
        {            
            Add(_Time);            
			Add(_World);						
            //AddFramework(Regulus.Utility.Singleton<Map>.Instance);            
            Add(_Hall);  
            Add(Regulus.Utility.Singleton<Storage>.Instance);                        
        }

        public override void Shutdown()
        {
            Remove(Regulus.Utility.Singleton<Storage>.Instance);            
            Remove(_Hall);
            //RemoveFramework(Regulus.Utility.Singleton<Map>.Instance);
			Remove(_World);
            Remove(_Time);            
        }
    }
}
