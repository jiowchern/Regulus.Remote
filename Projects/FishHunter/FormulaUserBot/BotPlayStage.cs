// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BotPlayStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the BotPlayStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

#endregion

namespace FormulaUserBot
{
	internal class BotPlayStage
		: IStage
	{
		public event DoneCallback DoneEvent;

		private readonly Updater _HitHandlers;

		private readonly TimeCounter _HitTime;

		private readonly IFishStage _Stage;

		public BotPlayStage(IFishStage fish_stage)
		{
			_HitHandlers = new Updater();
			_HitTime = new TimeCounter();
			_Stage = fish_stage;
		}

		void IStage.Leave()
		{
			_HitHandlers.Shutdown();
		}

		void IStage.Enter()
		{
		}

		void IStage.Update()
		{
			_HitHandlers.Working();
			if (_HitTime.Second > HitHandler.Interval)
			{
				// var totalHits = (byte)Regulus.Utility.Random.Next(1, 1000);
				var totalHits = (byte)1;
				for (var i = 0; i < totalHits; ++i)
				{
					_HitRequest(totalHits);
				}

				_HitTime.Reset();
			}
		}

		public delegate void DoneCallback();

		private void _HitRequest(byte total_hits)
		{
			var request = new HitRequest();

			request.FishID = (short)Random.Instance.NextInt(0, 32767);
			request.FishOdds = (short)Random.Instance.NextInt(1, 1000);

			request.FishStatus = Random.Instance.NextEnum<FISH_STATUS>();
			request.FishType = (byte)Random.Instance.NextInt(1, 99);
			request.TotalHits = total_hits;
			request.HitCnt = (short)Random.Instance.NextInt(1, request.TotalHits);
			request.TotalHitOdds = (short)Random.Instance.NextInt(0, 32767);
			request.WepBet = (short)Random.Instance.NextInt(1, 10000);
			request.WepID = (short)Random.Instance.NextInt(0, 32767);
			request.WepOdds = (short)Random.Instance.NextInt(1, 10000);
			request.WepType = 1;

			var hitHandler = new HitHandler(_Stage, request);
			_HitHandlers.Add(hitHandler);
		}
	}
}