// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Account.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Account type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Linq;

using ProtoBuf;

using Regulus.CustomType;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Common.Datas
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
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public string Name { get; set; }

		[ProtoMember(3)]
		public string Password { get; set; }

		[ProtoMember(4)]
		public Flag<COMPETENCE> Competnces { get; set; }

		public Account()
		{
			this.Competnces = new Flag<COMPETENCE>();
			this.Id = Guid.NewGuid();
			this.Name = this.Id.ToString();
			this.Password = this.Id.ToString();
		}

		public bool IsPassword(string password)
		{
			return this.Password == password;
		}

		public bool IsFormulaQueryer()
		{
			return this._HasCompetence(COMPETENCE.FORMULA_QUERYER);
		}

		private bool _HasCompetence(COMPETENCE competence)
		{
			return this.Competnces[competence];
		}

		public static Flag<COMPETENCE> AllCompetnce()
		{
			var flags = EnumHelper.GetEnums<COMPETENCE>().ToArray();
			var f = flags[0];
			return new Flag<COMPETENCE>(flags);
		}

		public bool HasCompetnce(COMPETENCE cOMPETENCE)
		{
			return this._HasCompetence(cOMPETENCE);
		}

		public bool IsPlayer()
		{
			return this._HasCompetence(COMPETENCE.GAME_PLAYER);
		}
	}
}