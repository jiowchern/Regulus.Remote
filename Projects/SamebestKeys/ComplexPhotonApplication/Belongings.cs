using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Belongings : IBelongings , Regulus.Utility.IUpdatable
    {
        IStorage _Storage;
        Guid _Owner;

        int _Coins;

        public Belongings(IStorage storage , Guid owner)
        {
            _Storage = storage;
            _Owner = owner;
        }                

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Coins = _Storage.QueryConsumptionCoins(_Owner);            
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }


        Remoting.Value<int> IBelongings.AddCoins(int coins)
        {
            _Add(coins);
            return _Coins;
        }

        private void _Add(int coins)
        {
            if (_Coins + coins > 0)
            {
                _Storage.AddConsumptionCoins(_Owner, coins);
                _Coins = _Storage.QueryConsumptionCoins(_Owner);

                _CoinsEvent(_Coins);
            }            
        }


        Remoting.Value<int> IBelongings.QueryCoins()
        {            
            return _Coins;
        }
        Remoting.Value<int> IBelongings.QueryCash()
        {
            return 0;
        }
        event Action<int> _CoinsEvent;
        event Action<int> IBelongings.CoinsEvent
        {
            add { _CoinsEvent += value; }
            remove  { _CoinsEvent -= value; }
        }

        event Action<int> _CashEvent;
        event Action<int> IBelongings.CashEvent
        {
            add { _CashEvent += value; }
            remove { _CashEvent -= value; }
        }

        
    }
}
