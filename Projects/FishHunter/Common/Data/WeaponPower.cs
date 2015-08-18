using System.Collections.Generic;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{

    [ProtoContract]
    public class FishPocket
	{

        [ProtoContract]
        public class Item
		{
            [ProtoMember(1)]
            public FISH_TYPE FishType;

            //隨機
            [ProtoMember(2)]
            public WEAPON_TYPE[] RandomWeapons;

            //必出
            [ProtoMember(3)]
            public WEAPON_TYPE CertainWeapons;

            [ProtoMember(4)]
            public int KillCount;

            [ProtoMember(5)]
            public int Power;

            [ProtoMember(6)]
            public int WinScore;
		}
        [ProtoMember(1)]
        public Item[] Items { get; set; }

		public FishPocket()
		{
			var items = new List<Item>();

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

                items.Add(fti);
			}

            items.Add(
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

            items.Add(
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

            items.Add(
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

            items.Add(
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

            items.Add(
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
			
			items.Add(
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

		    Items = items.ToArray();
		}
	}

	
}
