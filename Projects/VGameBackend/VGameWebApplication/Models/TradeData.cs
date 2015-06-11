using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VGameWebApplication.Models
{
    /// <summary>
    /// view model
    /// </summary>
    public class TradeData
    {
        public class Data
        {
            public Data()
            {

            }
            public Data(Guid id, string name, int money)
            {
                OwnerId = id;
                Name = name;
                Money = money;
                Deposit = 0;
            }

            public Guid OwnerId { get; set; }

            public string Name { get; set; }
            
            public int Money { get; set; }
            
            public int Deposit { get; set; }
        }

        public List<Data> Datas { get; set; }
        
        public void Add(Guid id, string name, int money)
        {
            Datas.Add(new Data(id, name, money));
        }
        public TradeData()
        {
            Datas = new List<Data>();
        }
    }
}