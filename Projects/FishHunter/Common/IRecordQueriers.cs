// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRecordQueriers.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IRecordQueriers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;

#endregion

namespace VGame.Project.FishHunter.Common
{
	public interface IRecordQueriers
	{
		Value<Record> Load(Guid id);

		void Save(Record record);
	}


	public interface ITradeNotes
	{
		Value<TradeNotes> Find(Guid id);

		Value<TradeNotes> Load(Guid id);

		Value<bool> Write(TradeNotes.TradeData data);

		Value<int> GetTotalMoney(Guid id);
	}
}