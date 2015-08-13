using System.Collections.Generic;

namespace VGame.Project.FishHunter.Common.Data
{
	

	public class FishCorrespondenceItem
	{
		public class FishToItem
		{
			public FISH_TYPE FishType;

			public WEAPON_TYPE[] OwnWeapons;

			public int KillCount;

			public int Power;

			public int WinScore;
		}

		public List<FishToItem> FishToItems { get; private set; }

		public FishCorrespondenceItem()
		{
			FishToItems = new List<FishToItem>();

			for(var i = FISH_TYPE.ORDINARY_FISH_BEGIN; i < FISH_TYPE.SCREEN_BOMB; ++i)
			{
				var fti = new FishToItem
				{
					FishType = i, 
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER
					}
				};

				FishToItems.Add(fti);
			}

			FishToItems.Add(
				new FishToItem
				{
					FishType = FISH_TYPE.SCREEN_BOMB,
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						WEAPON_TYPE.SCREEN_BOMB
					}
				});

			FishToItems.Add(
				new FishToItem
				{
					FishType = FISH_TYPE.THUNDER_BOMB,
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						WEAPON_TYPE.THUNDER_BOMB
					}
				});

			FishToItems.Add(
				new FishToItem
				{
					FishType = FISH_TYPE.FIRE_BOMB,
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						WEAPON_TYPE.FIRE_BOMB
					}
				});

			FishToItems.Add(
				new FishToItem
				{
					FishType = FISH_TYPE.DAMAGE_BALL,
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						WEAPON_TYPE.DAMAGE_BALL
					}
				});

			FishToItems.Add(
				new FishToItem
				{
					FishType = FISH_TYPE.OCTOPUS_BOMB,
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						WEAPON_TYPE.OCTOPUS_BOMB
					}
				});

			FishToItems.Add(
				new FishToItem
				{
					FishType = FISH_TYPE.BIG_OCTOPUS_BOMB,
					KillCount = 0,
					WinScore = 0,
					OwnWeapons = new[]
					{
						WEAPON_TYPE.SUPER_BOMB, 
						WEAPON_TYPE.ELECTRIC_NET, 
						WEAPON_TYPE.FREE_POWER, 
						WEAPON_TYPE.BIG_OCTOPUS_BOMB
					}
				});
		}
	}

	
}
