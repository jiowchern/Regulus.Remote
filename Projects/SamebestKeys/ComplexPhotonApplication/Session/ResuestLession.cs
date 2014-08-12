using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Session
{

    struct Lession
    {
        public readonly static Lession[] Lessions = 
        {
            new Lession() { Name = "apple-1" , Coin = 100 },
            new Lession() { Name = "apple-2" , Coin = 200 },
            new Lession() { Name = "apple-3" , Coin = 300 },
            new Lession() { Name = "apple-4" , Coin = 400 },
            new Lession() { Name = "apple-5" , Coin = 500 },
            new Lession() { Name = "apple-6" , Coin = 600 },
        };
        public string Name;
        public int Coin;
    }

    class RequestLession
    {
        public string Lession { get; set; }
        public string Name { get; set; }

        public Remoting.Value<bool> Answer;
    }
}
