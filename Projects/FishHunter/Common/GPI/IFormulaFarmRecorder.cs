using System;
using System.Net.Configuration;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IFormulaFarmRecorder
	{
		Value<FishFarmData> Load(int farm_id);

        Value<bool> Save(FishFarmData data);
	}
}
