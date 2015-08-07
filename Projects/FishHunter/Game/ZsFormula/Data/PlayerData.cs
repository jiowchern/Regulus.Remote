// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the PlayerVisitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class PlayerVisitor
	{
		private readonly List<PlayerRecord> _Players;

		public PlayerRecord FocusPlayer { get; private set; }

		public PlayerVisitor(Guid player_id, int stage_id)
		{
			_Players = new List<PlayerRecord>();
			
			QueryPlayer(player_id, stage_id);
		}

		private PlayerRecord _FindPlayer(Guid player_id)
		{
			return _Players.Find(x => x.Id == player_id);
		}

		/// <summary>
		/// 找不到 自動加入
		/// </summary>
		/// <param name="player_id"></param>
		/// <param name="stage_id"></param>
		/// <returns></returns>
		public StageRecord QueryPlayer(Guid player_id, int stage_id)
		{
			FocusPlayer  = _Players.Find(x => x.Id == player_id);
			if (FocusPlayer == null)
			{
				_Players.Add(new PlayerRecord(player_id));
			}

			var record = FocusPlayer.FindStageRecord(stage_id);

			if (record != null)
			{
				return record;
			}

			record = new StageRecord(stage_id);

			_FindPlayer(player_id).StageRecord.Add(record);

			return record;
		}
	}
}