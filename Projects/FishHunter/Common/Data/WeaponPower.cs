
using System.Linq;


using ProtoBuf;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
    /// <summary>
    /// 魚的口袋，有什麼寶物
    /// </summary>
    [ProtoContract]
	public class FishPocket
	{
		[ProtoContract]
		public class Item
		{
		    [ProtoMember(1)]
		    public FISH_TYPE FishType { get; set; }

		    /// <summary>
		    /// 隨機武器
		    /// </summary>
		    [ProtoMember(2)]
		    public WEAPON_TYPE[] RandomWeapons { get; set; }

		    /// <summary>
		    /// 必出武器
		    /// </summary>
		    [ProtoMember(3)]
		    public WEAPON_TYPE CertainWeapons { get; set; }

		    [ProtoMember(4)]
		    public int KillCount { get; set; }

		    [ProtoMember(5)]
		    public int Power { get; set; }

		    [ProtoMember(6)]
		    public int WinScore { get; set; }
		}

		[ProtoMember(1)]
		public Item[] Items { get; set; }

		public FishPocket()
		{
			var items =
				EnumHelper.GetEnums<FISH_TYPE>()
						  .Where(x => x >= FISH_TYPE.TROPICAL_FISH && x <= FISH_TYPE.EAT_FISH_CRAZY)
						  .Select(
							  i => new Item
							  {
								  FishType = i,
								  KillCount = 0,
								  WinScore = 0,
								  RandomWeapons = new[]
								  {
									  WEAPON_TYPE.SUPER_BOMB,
									  WEAPON_TYPE.ELECTRIC_NET,
									  WEAPON_TYPE.FREE_POWER
								  },
								  CertainWeapons = WEAPON_TYPE.INVALID
							  }).ToList();
			items.Add(
				new Item
				{
					FishType = FISH_TYPE.SPECIAL_SCREEN_BOMB,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB,
						WEAPON_TYPE.ELECTRIC_NET,
						WEAPON_TYPE.FREE_POWER
					},
					CertainWeapons = WEAPON_TYPE.SCREEN_BOMB
				});

			items.Add(
				new Item
				{
					FishType = FISH_TYPE.SPECIAL_THUNDER_BOMB, 
					KillCount = 0, 
					WinScore = 0, 
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER
					}, 
					CertainWeapons = WEAPON_TYPE.THUNDER_BOMB
				});

			items.Add(
				new Item
				{
					FishType = FISH_TYPE.SPECIAL_FIRE_BOMB, 
					KillCount = 0, 
					WinScore = 0, 
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER
					}, 
					CertainWeapons = WEAPON_TYPE.FIRE_BOMB
				});

			items.Add(
				new Item
				{
					FishType = FISH_TYPE.SPECIAL_DAMAGE_BALL, 
					KillCount = 0, 
					WinScore = 0, 
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER
					}, 
					CertainWeapons = WEAPON_TYPE.DAMAGE_BALL
				});

			items.Add(
				new Item
				{
					FishType = FISH_TYPE.SPECIAL_OCTOPUS_BOMB, 
					KillCount = 0, 
					WinScore = 0, 
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER
					}, 
					CertainWeapons = WEAPON_TYPE.OCTOPUS_BOMB
				});

			items.Add(
				new Item
				{
					FishType = FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB, 
					KillCount = 0, 
					WinScore = 0, 
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER
					}, 
					CertainWeapons = WEAPON_TYPE.BIG_OCTOPUS_BOMB
				});

			Items = items.ToArray();
		}
	}
}
