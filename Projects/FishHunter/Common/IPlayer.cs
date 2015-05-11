using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IPlayer
    {
        Regulus.Remoting.Value<int> RequestBullet();

        Regulus.Remoting.Value<int> Hit(int bullet , int[] fishids);

        void Quit();

        event Action<int> DeathFishEvent;
        event Action<int> MoneyEvent;
    }
}
