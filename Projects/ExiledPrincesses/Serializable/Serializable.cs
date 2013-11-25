using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
    
	[Serializable]
	public class AccountInfomation
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public Guid Id { get; set; }
        public GameRecord Record { get; set; }
	}

    [Serializable]
    public class GameRecord
    {
        public GameRecord()
        {
            Actors = new ActorInfomation[] { };
        }
        public ActorInfomation[] Actors { get; set; }
        public string Map { get; set; }
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
        Pub,
        Adventure,
        Battle,
    }

	[Serializable]
	public enum LoginResult
	{
        Success,
        Fail,
        Repeat
	}
    
    
}