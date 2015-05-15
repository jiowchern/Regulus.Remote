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
}
