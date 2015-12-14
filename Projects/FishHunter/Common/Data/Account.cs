using System;
using System.Linq;


using ProtoBuf;


using Regulus.CustomType;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	[ProtoContract]
	public class Account
	{
		public enum COMPETENCE
		{
			[EnumDescription("帳號管理")]
			ACCOUNT_MANAGER, 

			[EnumDescription("算法查詢")]
			FORMULA_QUERYER, 

			[EnumDescription("遊戲體驗")]
			GAME_PLAYER, 

			[EnumDescription("後端管理")]
			BACKEND_MANAGER, 

			[EnumDescription("算法記錄")]
			FORMULA_RECORDER, 

			[EnumDescription("帳號查詢")]
			ACCOUNT_FINDER
		};

		

		[ProtoMember(1)]
		public long Id { get; set; }
		
		[ProtoMember(2)]
		public string Name { get; set; }

		[ProtoMember(3)]
		public string Password { get; set; }

		[ProtoMember(4)]
		public Flag<COMPETENCE> Competnces { get; set; }

		[ProtoMember(5)]
		public Guid Guid { get; set; }

		public Account()
		{
			Competnces = new Flag<COMPETENCE>();
			Guid = Guid.NewGuid();
			Name = Id.ToString();
			Password = Id.ToString();
		}

		public bool IsPassword(string password)
		{
			return Password == password;
		}

		public bool IsFormulaQueryer()
		{
			return _HasCompetence(COMPETENCE.FORMULA_QUERYER);
		}

		private bool _HasCompetence(COMPETENCE competence)
		{
			return Competnces[competence];
		}

		public static Flag<COMPETENCE> AllCompetnce()
		{
			var flags = EnumHelper.GetEnums<COMPETENCE>().ToArray();			
			return new Flag<COMPETENCE>(flags);
		}

		public bool HasCompetnce(COMPETENCE cOMPETENCE)
		{
			return _HasCompetence(cOMPETENCE);
		}

		public bool IsPlayer()
		{
			return _HasCompetence(COMPETENCE.GAME_PLAYER);
		}
	}
}
