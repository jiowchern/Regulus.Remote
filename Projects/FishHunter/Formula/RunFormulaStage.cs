using Regulus.Collection;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Storage;

namespace VGame.Project.FishHunter.Formula
{
	internal class RunFormulaStage : IStage
	{
		public delegate void DoneCallback();

		public event DoneCallback DoneEvent;

	    

	    private readonly Queue<ISoulBinder> _Binders;

		private readonly Center _Center;

		private readonly Updater _Updater;

		private ICore _Core
		{
			get { return _Center; }
		}

		public RunFormulaStage( ExpansionFeature expansion_feature, Queue<ISoulBinder> binders)
		{
		    
		    _Binders = binders;
			_Updater = new Updater();
			_Center = new Center(expansion_feature);
		}

		void IStage.Enter()
		{
		    
            _Updater.Add(_Center);
		}
	    

	    void IStage.Leave()
		{
			_Updater.Shutdown();
            
        }

		void IStage.Update()
		{
			_Updater.Working();

			foreach(var binder in _Binders.DequeueAll())
			{
				_Core.AssignBinder(binder);
			}

            
		}
	}
}
