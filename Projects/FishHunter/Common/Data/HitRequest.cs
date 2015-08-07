using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{

	public class RequsetFishData
	{
		/// <summary>
		///		鱼的编号，识别是那只鱼用的。
		/// </summary>
		[ProtoMember(1)]
		public int FishID;
		
		/// <summary>
		///     鱼的种类。一般鱼从1~99。特殊鱼从201开始。。。
		/// </summary>
		[ProtoMember(2)]
		public FISH_TYPE FishType;

		/// <summary>
		///      鱼的状态。
		/// </summary>
		[ProtoMember(3)]
		public FISH_STATUS FishStatus;

		/// <summary>
		///      鱼的倍数。应该从1~1000。目前只预留到1000倍。
		/// </summary>
		[ProtoMember(4)]
		public int FishOdds;

		public RequsetFishData()
		{
			FishOdds = 0;
		}
	}

	public struct RequestWeaponData
	{
		/// <summary>
		///     子彈编号，识别是那颗子弹（网）用的。
		/// </summary>
		[ProtoMember(1)]
		public int WepID;

		/// <summary>
		///      武器形态。一般子弹为“1”。特殊子弹从“2”开始。。。
		/// </summary>
		[ProtoMember(2)]
		public WEAPON_TYPE WeaponType;

		/// <summary>
		///     武器分数，就是该子弹的玩家押注分数。从1~10000。
		/// </summary>
		[ProtoMember(3)]
		public int WepBet;


		/// <summary>
		///     武器倍数，就是该子弹的玩家押注倍数。从1~10000。
		/// </summary>
		[ProtoMember(4)]
		public int WepOdds;

		/// <summary>
		///     武器同时击中的鱼总数量。应该从1~1000。
		/// </summary>
		[ProtoMember(5)]
		public int TotalHits;

		/// <summary>
		///     武器同时击中的鱼倍数总和。
		/// </summary>
		[ProtoMember(6)]
		public int TotalHitOdds;
	}


	/// <summary>
	/// 新版的封包定義
	/// </summary>
	
	[ProtoContract]
	public class HitRequest
	{
		public HitRequest()
		{
			
		}

		public HitRequest(RequsetFishData[] fish_datas, RequestWeaponData weapon_data)
		{
			FishDatas = fish_datas;
			WeaponData = weapon_data;
		}

		[ProtoMember(1)]
		public RequsetFishData[] FishDatas;

		[ProtoMember(2)]
		public RequestWeaponData WeaponData;
	}

	/// <summary>
	///     判定請求
	/// </summary>
	//[ProtoContract]
	//public struct HitRequest
	//{
		/// <summary>
		///     子弹编号，识别是那颗子弹（网）用的。
		/// </summary>
		//[ProtoMember(1)]
		//public short WepID;

		///// <summary>
		/////     子弹形态。一般子弹为“1”。特殊子弹从“2”开始。。。
		///// </summary>
		//[ProtoMember(2)]
		////public byte WepType;
		//public WEAPON_TYPE WeaponType;

		///// <summary>
		/////     子弹分数，就是该子弹的玩家押注分数。从1~10000。
		///// </summary>
		//[ProtoMember(3)]
		//public short WepBet;

		///// <summary>
		/////     子弹倍数，就是该子弹的玩家押注倍数。从1~10000。
		///// </summary>
		//[ProtoMember(4)]
		//public short WepOdds;

		///// <summary>
		/////     鱼的编号，识别是那只鱼用的。
		///// </summary>
		//[ProtoMember(5)]
		////public short FishID;
		//public short FishID;

		///// <summary>
		/////     鱼的种类。一般鱼从1~99。特殊鱼从201开始。。。
		///// </summary>
		//[ProtoMember(6)]
		////public byte FishType;
		//public FISH_TYPE FishType;

		///// <summary>
		/////     鱼的倍数。应该从1~1000。目前只预留到1000倍。
		///// </summary>
		//[ProtoMember(7)]
		//public short FishOdds;

		///// <summary>
		/////     鱼的状态。
		///// </summary>
		//[ProtoMember(8)]
		//public FISH_STATUS FishStatus;

		///// <summary>
		/////     该网同时击中的鱼总数量。应该从1~1000。
		///// </summary>
		//[ProtoMember(9)]
		//public short TotalHits;

		///// <summary>
		/////     同时击中的排序序号，应该是从1~ TotalHits。
		///// </summary>
		//[ProtoMember(10)]
		//public short HitCnt;

		///// <summary>
		/////     该网同时击中的鱼倍数总和。
		///// </summary>
		//[ProtoMember(11)]
		//public short TotalHitOdds;
	//}
}