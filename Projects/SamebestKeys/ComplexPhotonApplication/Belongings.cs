using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Belongings : IBelongings
    {
        IStorage _Storage;
        Guid _Owner;

        public Belongings(IStorage storage , Guid owner)
        {
            _Storage = storage;
            _Owner = owner;
        }
        Remoting.Value<int> IBelongings.QueryCoins()
        {
            return _Storage.QueryConsumptionCoins(_Owner);
        }


        void IBelongings.AddCoins(int coins)
        {
            _Storage.AddConsumptionCoins(_Owner, coins);
        }
    }
}
