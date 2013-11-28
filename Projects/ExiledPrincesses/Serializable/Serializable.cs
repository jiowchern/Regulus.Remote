using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
    [Serializable]
    public enum Strategy
    {
        None,
        Sword,
        Staff,
        Ax,
        Shield
    };
    
	[Serializable]
	public class AccountInfomation
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public Guid Id { get; set; }
        public GameRecord Record { get; set; }
	}

    [Serializable]
    public class Contingent
    {
        public enum FormationType
        {
            Auxiliary,
            Defensive,
        }
        public FormationType Formation { get; set; }
        public Guid[] Members { get; set; }
    }

    [Serializable]
    public class GameRecord
    {
        public GameRecord()
        {
            Actors = new ActorInfomation[] { };
        }
        public ActorInfomation[] Actors { get; set; }
        public string Tone { get; set; }
        public Contingent Contingent { get; set; }


        public ActorInfomation[] GetContingentActors()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class PrototypeActor
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Int { get; set; }
        public int Dex { get; set; }
        public int Exp { get; set; }
        public Strategy Specializes { get; set; } 
    }

    [Serializable]
    public class ActorInfomation
    {        
        public Guid Id { get; set; }
        public int Prototype { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }        
    }

    [Serializable]
    public enum UserStatus
    {
        None,
        Verify,
        Tone,
        Adventure,        
    }

	[Serializable]
	public enum LoginResult
	{
        Success,
        Fail,
        Repeat
	}
    
    
}