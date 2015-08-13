using System;
using System.Collections.Generic;
using System.Linq;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	[ProtoContract]
	public class TradeNotes
	{
		[ProtoContract]
		public class TradeData
		{
			[ProtoMember(1)]
			public Guid BuyerId;

			[ProtoMember(5)]
			public DateTime CreateTime;

			[ProtoMember(6)]
			public bool IsUsed;

			[ProtoMember(3)]
			public int Money;

			[ProtoMember(2)]
			public Guid SellerId;

			[ProtoMember(4)]
			public int SerialNo;

			public TradeData()
			{
			}

			public TradeData(Guid seller_id, Guid buyer_id, int money, int s_Id)
			{
				SellerId = seller_id;
				BuyerId = buyer_id;
				Money = money;
				SerialNo = s_Id;
				CreateTime = new DateTime();
				IsUsed = false;
			}
		}

		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public Guid Owner { get; set; }

		[ProtoMember(3)]
		public List<TradeData> TradeDatas { get; set; }

		public TradeNotes()
		{
			Id = Guid.NewGuid();
			TradeDatas = new List<TradeData>();
		}

		public TradeNotes(Guid id)
		{
			Id = Guid.NewGuid();
			Owner = id;
			TradeDatas = new List<TradeData>();
		}

		public int GetTotalMoney()
		{
			return (from a in TradeDatas where a.IsUsed == false select a.Money).Sum();
		}

		public void SetTradeIsUsed()
		{
			foreach(var a in TradeDatas)
			{
				a.IsUsed = true;
			}
		}
	}
}
