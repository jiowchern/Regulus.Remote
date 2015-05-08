using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IPlayer
    {
        void Hit(int fishid,short bullet_score);

        void Quit();

        event Action<int> DeathFishEvent;
        event Action<int> MoneyEvent;
    }
}
