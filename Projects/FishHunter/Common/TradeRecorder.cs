using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
     
    [ProtoBuf.ProtoContract]
    public class TradeNotes
    {
        [ProtoBuf.ProtoMember(1)]
        public Guid OwnerId { get; set; }

        [ProtoBuf.ProtoMember(2)]
        List<Data.TradeData> _TradeDatas ;

        public List<Data.TradeData> TradeData { get { return _TradeDatas; } }


        public TradeNotes()
        {
            _TradeDatas = new List<Data.TradeData>();
        }
        public TradeNotes(Guid id)
        {
            OwnerId = id;
            _TradeDatas = new List<Data.TradeData>();
        }

        public void SavingTrade(Data.TradeData data)
        {
            _TradeDatas.Add(data);
        }

        public int GetTotalDeposit()
        {
            return (from a in _TradeDatas where a.IsUsed == false select a.Money).Sum();
        }

        public void SetTradeIsUsed()
        {
            foreach (var a in _TradeDatas)
            {
                a.IsUsed = true;
            }
        }
    }
}


namespace VGame.Project.FishHunter.Data
{
    [ProtoBuf.ProtoContract]
    public class TradeData
    {
        public TradeData(Guid seller_id, Guid buyer_id, int money, int s_Id)
        {
            SellerId = seller_id;
            BuyerId = buyer_id;
            Money = money;
            SerialNo = s_Id;
            CreateTime = new DateTime();
            IsUsed = false;
        }

        [ProtoBuf.ProtoMember(1)]
        public Guid BuyerId;
          
        [ProtoBuf.ProtoMember(2)]
        public Guid SellerId;
          
        [ProtoBuf.ProtoMember(3)]
        public int Money;
          
        [ProtoBuf.ProtoMember(4)]
        public int SerialNo;

        [ProtoBuf.ProtoMember(5)]
        public DateTime CreateTime;

        [ProtoBuf.ProtoMember(6)]
        public bool IsUsed;
    }
}
