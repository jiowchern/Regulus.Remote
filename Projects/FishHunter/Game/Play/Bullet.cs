using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class Bullet
    {
        static int _Sn;
        public int Id { get; set; }
        public Bullet(int id)
        {
            Id = id;
        }
        public Bullet()
        {
            Id = ++_Sn;
        }
    }
}
