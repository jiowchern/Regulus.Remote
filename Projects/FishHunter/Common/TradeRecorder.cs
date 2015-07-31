// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TradeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the TradeNotes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

using ProtoBuf;

#endregion

namespace VGame.Project.FishHunter.Common
{
	[ProtoContract]
	public class TradeNotes
	{
		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public Guid Owner { get; set; }

		[ProtoMember(3)]
		public List<TradeData> TradeDatas { get; set; }

		public TradeNotes()
		{
			this.Id = Guid.NewGuid();
			this.TradeDatas = new List<TradeData>();
		}

		public TradeNotes(Guid id)
		{
			this.Id = Guid.NewGuid();
			this.Owner = id;
			this.TradeDatas = new List<TradeData>();
		}

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
				this.SellerId = seller_id;
				this.BuyerId = buyer_id;
				this.Money = money;
				this.SerialNo = s_Id;
				this.CreateTime = new DateTime();
				this.IsUsed = false;
			}
		}

		public int GetTotalMoney()
		{
			return (from a in this.TradeDatas where a.IsUsed == false select a.Money).Sum();
		}

		public void SetTradeIsUsed()
		{
			foreach (var a in this.TradeDatas)
			{
				a.IsUsed = true;
			}
		}
	}
}