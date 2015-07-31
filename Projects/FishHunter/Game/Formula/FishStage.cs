// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FishStage.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the FishStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Extension;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;

using Random = Regulus.Utility.Random;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	internal class FishStage : IFishStage
	{
		private event Action<string> _OnHitExceptionEvent;

		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly byte _FishStage;

		private readonly HitBase _Formula;

		public FishStage(long account, int stage_id)
		{
			this._Formula = new HitTest(Random.Instance);
			this._AccountId = account;
			this._FishStage = (byte)stage_id;
		}

		event Action<string> IFishStage.HitExceptionEvent
		{
			add { this._OnHitExceptionEvent += value; }
			remove { this._OnHitExceptionEvent -= value; }
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

		byte IFishStage.FishStage
		{
			get { return this._FishStage; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var response = this._Formula.Request(request);

			this._OnHitResponseEvent.Invoke(response);

			this._MakeLog(request, response);
		}

		private void _MakeLog(HitRequest request, HitResponse response)
		{
			var format = "Player:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

			var log = string.Format(
				format, 
				this._AccountId, 
				this._FishStage, 
				request.ShowMembers(" "), 
				response.ShowMembers(" "));

			Singleton<Log>.Instance.WriteInfo(log);
		}
	}
}