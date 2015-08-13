using System;
using System.Net.Configuration;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IFormulaStageDataRecorder
	{
		Value<StageData> Load(int stage_id);

		Value<bool> Save(StageData data);
	}
}
