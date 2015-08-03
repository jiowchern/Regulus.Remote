// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSFishStage.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the CsFishStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;

using Random = Regulus.Utility.Random;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public class CsFishStage : IFishStage
	{
		private event Action<string> _OnHitExceptionEvent;

		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly byte _FishStage;

		private readonly HitBase _Formula;

		public CsFishStage(long player_id, byte stage_id)
		{
			this._Formula = new HitTest(Random.Instance);
			this._AccountId = player_id;
			this._FishStage = stage_id;
		}

		byte IFishStage.FishStage
		{
			get { return this._FishStage; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var response = this._Formula.Request(request);

			this._OnHitResponseEvent.Invoke(response);

			// this._MakeLog(request, response);
		}

		event Action<string> IFishStage.HitExceptionEvent
		{
			add { _OnHitExceptionEvent += value; }
			remove { _OnHitExceptionEvent -= value; }
		}

		event Action<HitResponse> IFishStage.HitResponseEvent
		{
			add { this._OnHitResponseEvent += value; }
			remove { this._OnHitResponseEvent -= value; }
		}

		long IFishStage.AccountId
		{
			get { return this._AccountId; }
		}
	}
}