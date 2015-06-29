using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
    public class ChancesTable<T>
    {
        private Data[] _Datas;
        public ChancesTable(Data[] datas)
        {
            if (datas.Length == 0)
                throw new ArgumentException("參數數量不可為0");
            _Datas = datas;
        }
        public class Data
        {
            public T Id { get; set; }

            public float Rate { get; set; }
        }

        public T Dice(float chances)
        {
            float minGate = 0.0f;
            foreach (var data in _Datas)
            {
                float gate = minGate + data.Rate;

                if (minGate <= chances && chances < gate)
                {
                    return data.Id;
                }
                minGate += data.Rate;
            }

            return _Datas.First().Id;
        }
    }
}
