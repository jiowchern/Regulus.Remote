using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IRecordQueriers
    {
        Regulus.Remoting.Value<Data.Record> Load(Guid id);

        void Save(Data.Record record);
    }
    

    public interface ITradeNotes
    {
        Regulus.Remoting.Value<Data.TradeNotes> Find(Guid id);
        
        Regulus.Remoting.Value<Data.TradeNotes> Load(Guid id);

        //Regulus.Remoting.Value<Data.TradeData> Saving(Guid id);
        Regulus.Remoting.Value<bool> Write(Data.TradeNotes.TradeData data);
    }
}
