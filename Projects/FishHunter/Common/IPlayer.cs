using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IPlayer
    {

        int WeaponOdds { get; }
        WEAPON Weapon { get; }

        Regulus.Remoting.Value<int> RequestBullet();
        Regulus.Remoting.Value<short> RequestFish();

        Regulus.Remoting.Value<int> Hit(int bullet , int[] fishids);

        

        void EquipWeapon(WEAPON weapon, int odds);

        void Quit();

        event Action<int> DeathFishEvent;
        event Action<int> MoneyEvent;
    }
}
