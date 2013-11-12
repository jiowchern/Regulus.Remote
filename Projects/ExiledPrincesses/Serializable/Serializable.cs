using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
    [Serializable]
    public class BattleSpeed
    {
        public int Speed;
        public string Name;
    }

    [Serializable]
    public class EnergyGroup
    {        
        public Energy Energy;
        public int Hp;
        public int Change;
        public int Round;
        public Guid Owner;
    }

    [Serializable]
    public class Energy
    {        
        public int Red { get; private set; }
        public int Yellow { get; private set; }
        public int Green { get; private set; }
        public int Power { get; private set; }
        int _TotalMax;

        public Energy(int total_max)
        {                
            _TotalMax = total_max;
        }
        public bool _CheckTotal()
        {
            return Red + Yellow + Green <= _TotalMax;
        }
        public void SubGreen()
        {
            if (Green > 0)
                Green--;
        }
        public void SubRed()
        {
            if (Red > 0)
                Red--;
        }
        public void SubYellow()
        {
            if (Yellow > 0)
                Yellow--;
        }
        public void SubPower()
        {
            if (Power > 0)
                Power--;
        }
        public bool IncRed()
        {
            if (_CheckTotal())
            {
                Red++;
                return true;
            }
            return false;
        }

        public bool IncYellow()
        {
            if (_CheckTotal())
            {
                Yellow++;
                return true;
            }
            return false;
        }
        public bool IncGreen()
        {
            if (_CheckTotal())
            {
                Green++;
                return true;
            }
            return false;
        }

        public bool IncPower()
        {
            if (Power == 0)
            {
                Power++;
                return true;
            }
            return false;
        }

        public bool Consume(int r, int y, int g, int p)
        {
            if (Check(r, y, g, p))
            {
                Red -= r;
                Yellow -= y;
                Green -= g;
                Power -= p;
                return true;
            }
            return false;
        }
        public bool Check(int r, int y, int g, int p)
        {
            return Red >= r && Yellow >= y && Green >= g && Power >= p;
        }
    };
    [Serializable]
    public class Pet
    {                
        public Guid Id { get; set; }
        public Guid Owner { get; set; }
        public string Name { get; set; }
        public Energy Energy { get; set; }
    }
	[Serializable]
	public class AccountInfomation
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public Guid Id { get; set; }
	}

    [Serializable]
    public class ActorInfomation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    [Serializable]
    public enum UserStatus
    {
        None,
        Verify,
        Parking,
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

    [Serializable]
    public enum BattlerSide
    { 
        Blue,Red
    }

    
    [Serializable]
    public class BattlerInfomation
    {
        public Guid Id;
        public BattlerSide Side;
    }

    [Serializable]
    public class BattleRequester
    {
        
        public BattleRequester()
        {
            Battlers = new List<BattlerInfomation>();
        }
        public List<BattlerInfomation> Battlers { get; private set; }
    }
}