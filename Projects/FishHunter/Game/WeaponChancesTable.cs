using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class WeaponChancesTable
    {
        private Data[] _Datas;

        public WeaponChancesTable(Data[] datas)
        {
            if (datas.Length == 0)
                throw new ArgumentException("參數數量不可為0");

            this._Datas = datas;
        }
        public class Data
        {
            public int Id { get; set; }

            public float Rate { get; set; }
        }

        internal int Dice(float chances)
        {
            float minGate = 0.0f;            
            foreach(var data in _Datas)
            {
                float gate = minGate + data.Rate;

                if(minGate <= chances &&  chances < gate )
                {
                    return data.Id;
                }
                minGate += data.Rate;
            }

            return _Datas.First().Id;
        }
    }
}
