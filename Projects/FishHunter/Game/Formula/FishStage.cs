#region Test_Region

using System;


using Regulus.Extension;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Datas;
using VGame.Project.FishHunter.Common.GPIs;


using Random = Regulus.Utility.Random;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	internal class ZsFishStage : IFishStage
	{
		public ZsFishStage(byte fish_stage, long account_id)
		{

			_FishStage = fish_stage;
			_AccountId = account_id;

			//read file get stage data


		}

		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly byte _FishStage;

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		event Action<HitResponse> IFishStage.OnHitResponseEvent
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		long IFishStage.AccountId
		{
			get { throw new NotImplementedException(); }
		}

		byte IFishStage.FishStage
		{
			get { throw new NotImplementedException(); }
		}

		void IFishStage.Hit(HitRequest request)
		{
			throw new NotImplementedException();
		}
	}

	internal class FishStage : IFishStage
	{
		private event Action<string> _OnHitExceptionEvent;

		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly byte _FishStage;

		private readonly HitBase _Formula;

		public FishStage(long account, int stage_id)
		{
			_Formula = new HitTest(Random.Instance);
			_AccountId = account;
			_FishStage = (byte)stage_id;
		}

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add { this._OnHitExceptionEvent += value; }
			remove { this._OnHitExceptionEvent -= value; }
		}

		event Action<HitResponse> IFishStage.OnHitResponseEvent
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
			var response = _Formula.Request(request);

			_OnHitResponseEvent.Invoke(response);

			_MakeLog(request, response);
		}

		private void _MakeLog(HitRequest request, HitResponse response)
		{
			var format = "Player:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

			var log = string.Format(
				format, 
				_AccountId, 
				_FishStage, 
				request.ShowMembers(" "), 
				response.ShowMembers(" "));

			Singleton<Log>.Instance.WriteInfo(log);
		}
	}
}