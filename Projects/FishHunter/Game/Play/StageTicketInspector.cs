// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageTicketInspector.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StageTicketInspector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace VGame.Project.FishHunter.Play
{
	internal class StageTicketInspector
	{
		private readonly StageGate _StageGate;

		private HashSet<Common.Data.Stage> _Current;

		private int _KillCount;

		public int[] PlayableStages
		{
			get { return (from stage in _Current select stage.Id).ToArray(); }
		}

		public StageTicketInspector(StageGate sg)
		{
			this._StageGate = sg;
			_Current = new HashSet<Common.Data.Stage>();
		}

		internal void Initial(Common.Data.Stage[] stages)
		{
			_Current = new HashSet<Common.Data.Stage>(stages);
			_Update();
		}

		internal void Kill(int count)
		{
			_KillCount += count;
			_Update();
		}

		internal void Pass(int stage)
		{
			_Current.Add(new Common.Data.Stage
			{
				Id = stage, 
				Pass = true
			});
			_Update();
		}

		private void _Update()
		{
			var passs = from stage in _Current where stage.Pass select stage.Id;
			foreach (var stage  in _StageGate.FindUnlockStage(passs, _KillCount))
			{
				_Current.Add(new Common.Data.Stage
				{
					Id = stage, 
					Pass = false
				});
			}
		}
	}
}