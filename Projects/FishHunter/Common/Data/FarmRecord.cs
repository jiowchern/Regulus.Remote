using System;

using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     漁場記錄
	/// </summary>
	[ProtoContract]
	public class FarmRecord
	{
		[ProtoMember(1)]
		public Guid Id { get; set; }

		/// <summary>
		///     漁場id
		/// </summary>
		[ProtoMember(2)]
		public int FarmId { get; set; }

		/// <summary>
		///     花費總合
		/// </summary>
		[ProtoMember(3)]
		public int TotalSpending { get; set; }

		/// <summary>
		///     贏分
		/// </summary>
		[ProtoMember(4)]
		public int WinScore { get; set; }

		/// <summary>
		///     遊戲次數
		/// </summary>
		[ProtoMember(5)]
		public int FireCount { get; set; }

		/// <summary>
		///     目前沒在用
		/// </summary>
		[ProtoMember(6)]
		public int WinFrequency { get; set; }

		/// <summary>
		///     記錄開啟好贏的次數
		/// </summary>
		[ProtoMember(7)]
		public int AsnTimes { get; set; }

		/// <summary>
		///     記錄開啟好贏的總贏分
		/// </summary>
		[ProtoMember(8)]
		public int AsnWin { get; set; }

		/// <summary>
		///     記錄打死的魚
		/// </summary>
		[ProtoMember(9)]
		public WeaponHitRecord[] WeaponHitRecords { get; set; }

		/// <summary>
		///     記錄得到的隨機寶物
		/// </summary>
		[ProtoMember(10)]
		public TreasureRecord[] RandomTreasures { get; set; }

		public FarmRecord(int farm_id)
		{
			Id = Guid.NewGuid();

			FarmId = farm_id;
			WeaponHitRecords = new WeaponHitRecord[0];
			RandomTreasures = new TreasureRecord[0];
		}

		public FarmRecord()
		{
			Id = Guid.NewGuid();
			WeaponHitRecords = new WeaponHitRecord[0];
			RandomTreasures = new TreasureRecord[0];
		}
	}
}
