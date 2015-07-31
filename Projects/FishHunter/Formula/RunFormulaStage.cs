// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunFormulaStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RunFormulaStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Collection;
using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	internal class RunFormulaStage : IStage
	{
		public event DoneCallback DoneEvent;

		private readonly Queue<ISoulBinder> _Binders;

		private readonly Center _Center;

		private readonly Updater _Updater;

		private ICore _Core
		{
			get { return this._Center; }
		}

		public RunFormulaStage(StorageController controller, Queue<ISoulBinder> binders) : this(controller)
		{
			// TODO: Complete member initialization        
			this._Binders = binders;
		}

		private RunFormulaStage(StorageController controller)
		{
			this._Updater = new Updater();
			this._Center = new Center(controller);
		}

		void IStage.Enter()
		{
			this._Updater.Add(this._Center);
		}

		void IStage.Leave()
		{
			this._Updater.Shutdown();
		}

		void IStage.Update()
		{
			this._Updater.Working();

			foreach (var binder in this._Binders.DequeueAll())
			{
				this._Core.AssignBinder(binder);
			}
		}

		public delegate void DoneCallback();
	}
}