using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IGameRecorder
	{
		Value<GamePlayerRecord> Load(Guid account_id);

		void Save(GamePlayerRecord game_player_record);
	}
}
