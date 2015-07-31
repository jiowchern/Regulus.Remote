// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TradeData.cs" company="">
//   
// </copyright>
// <summary>
//   view model
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

#endregion

namespace VGameWebApplication.Models
{
	/// <summary>
	///     view model
	/// </summary>
	public class TradeData
	{
		public List<Data> Datas { get; set; }

		public TradeData()
		{
			Datas = new List<Data>();
		}

		public class Data
		{
			public Guid OwnerId { get; set; }

			public string Name { get; set; }

			public int Money { get; set; }

			public int Deposit { get; set; }

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
		}

		public void Add(Guid id, string name, int money)
		{
			Datas.Add(new Data(id, name, money));
		}
	}
}