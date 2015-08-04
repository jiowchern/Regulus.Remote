using System;

using Regulus.Remoting;

using VGame.Project.FishHunter.Common.Datas;

namespace VGame.Project.FishHunter.Common.GPIs
{
	public interface ITradeNotes
	{
		Value<TradeNotes> Find(Guid id);

		Value<TradeNotes> Load(Guid id);

		Value<bool> Write(TradeNotes.TradeData data);

		Value<int> GetTotalMoney(Guid id);
	}
}