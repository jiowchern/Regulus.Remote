using System;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	// public delegate void HitResponseCallback(HitResponse response);
	// public delegate void HitExceptionCallback(string message);
	/// <summary>
	///     魚場
	/// </summary>
	public interface IFishStage
	{
		/// <summary>
		///     例外
		/// </summary>
		event Action<string> OnHitExceptionEvent;

		/// <summary>
		///     判定結果事件
		/// </summary>
		event Action<HitResponse> OnHitResponseEvent;

		/// <summary>
		/// 一次判定所有hit事件
		/// </summary>
		event Action<HitResponse[]> OnTotalHitResponseEvent;

		/// <summary>
		///     玩家id
		/// </summary>
		Guid AccountId { get; }

		/// <summary>
		///     魚場id
		/// </summary>
		int FishStage { get; }

		/// <summary>
		///     判定請求
		/// </summary>
		/// <param name="request"></param>
		void Hit(HitRequest request);
	}
}
