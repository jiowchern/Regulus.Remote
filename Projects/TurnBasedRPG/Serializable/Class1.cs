using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG.Serializable
{
    [Serializable]
    public class AccountInfomation 
    {
        

        public string Name { get; set; }
        public string Password{ get; set; }
        public Guid Id { get; set; }        
    }

    [Serializable]
    public class ActorInfomation
    {
        public string Name { get; set; }
    }

    [Serializable]
    public class DBActorInfomation : ActorInfomation
    {
        public Guid Owner { get; set; }
        public int TestData { get; set; }
    }

}
