using System.Collections.Generic;

namespace VGame.Project.FishHunter.Common.Data
{
	

	public class FishPocket
	{
		public class Item
		{
			public FISH_TYPE FishType;

			//隨機
			public WEAPON_TYPE[] RandomWeapons;
			
			//必出
			public WEAPON_TYPE CertainWeapons;

			public int KillCount;

			public int Power;

			public int WinScore;
		}

		public List<Item> Items { get; private set; }

		public FishPocket()
		{
			Items = new List<Item>();

			for(var i = FISH_TYPE.ORDINARY_FISH_BEGIN; i < FISH_TYPE.SCREEN_BOMB; ++i)
			{
				var fti = new Item
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
				};

				Items.Add(fti);
			}

			Items.Add(
				new Item
				{
					FishType = FISH_TYPE.SCREEN_BOMB,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
					},
					CertainWeapons = WEAPON_TYPE.SCREEN_BOMB
				});

			Items.Add(
				new Item
				{
					FishType = FISH_TYPE.THUNDER_BOMB,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						
					},
					CertainWeapons = WEAPON_TYPE.THUNDER_BOMB
				});

			Items.Add(
				new Item
				{
					FishType = FISH_TYPE.FIRE_BOMB,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
					},
					CertainWeapons = WEAPON_TYPE.FIRE_BOMB
				});

			Items.Add(
				new Item
				{
					FishType = FISH_TYPE.DAMAGE_BALL,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
					},
					CertainWeapons = WEAPON_TYPE.DAMAGE_BALL
				});

			Items.Add(
				new Item
				{
					FishType = FISH_TYPE.OCTOPUS_BOMB,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB,
						WEAPON_TYPE.ELECTRIC_NET,
						WEAPON_TYPE.FREE_POWER,
					},
					CertainWeapons = WEAPON_TYPE.OCTOPUS_BOMB
				});
			
			Items.Add(
				new Item
				{
					FishType = FISH_TYPE.BIG_OCTOPUS_BOMB,
					KillCount = 0,
					WinScore = 0,
					RandomWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
					},
					CertainWeapons = WEAPON_TYPE.BIG_OCTOPUS_BOMB
				});
		}
	}

	
}
