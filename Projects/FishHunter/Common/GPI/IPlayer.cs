// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlayer.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IPlayer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

#endregion

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IPlayer
	{
		event Action<int> DeathFishEvent;

		event Action<int> MoneyEvent;

		int WeaponOdds { get; }

		WEAPON_TYPE WeaponType { get; }

		Value<int> RequestBullet();

		Value<short> RequestFish();

		Value<int> Hit(int bullet, int[] fishids);

		void EquipWeapon(WEAPON_TYPE weapon_type, int odds);

		void Quit();
	}
}