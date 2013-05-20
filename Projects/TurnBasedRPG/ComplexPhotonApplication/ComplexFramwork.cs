using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class ComplexFramwork : Samebest.Remoting.PhotonExpansion.PhotonFramework 
    {        
        Regulus.Project.TurnBasedRPG.World _World;
        Regulus.Project.TurnBasedRPG.Hall _Hall;

        public ComplexFramwork()
        {
            _Hall = new Hall();
        }

        public override void ObtainController(Samebest.Remoting.Soul.SoulProvider provider)
        {
            _Hall.PushUser(new User(provider , _Hall));            
        }

        public override void Launch()
        {
            AddFramework(_Hall);            
            AddFramework(Samebest.Utility.Singleton<Storage>.Instance);            
        }

        public override void Shutdown()
        {
            RemoveFramework(_Hall);            
            RemoveFramework(Samebest.Utility.Singleton<Storage>.Instance);            
        }
    }
}
