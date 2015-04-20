using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class RunFormulaStage : Regulus.Utility.IStage
    {
        public delegate void DoneCallback();
        public event DoneCallback DoneEvent;
        VGame.Project.FishHunter.Formula.Center _Center;

        Regulus.Utility.ICore _Core { get { return _Center; } }
        Regulus.Utility.Updater _Updater;
        
        private Regulus.Collection.Queue<Regulus.Remoting.ISoulBinder> _Binders;
        

        private RunFormulaStage(Formula.StorageController controller)
        {
            _Updater = new Regulus.Utility.Updater();
            _Center = new Formula.Center(controller);
            
        }

        public RunFormulaStage(Formula.StorageController controller, Regulus.Collection.Queue<Regulus.Remoting.ISoulBinder> binders) : this(controller)
        {
            // TODO: Complete member initialization        
            this._Binders = binders;
        }

        
        void Regulus.Utility.IStage.Enter()
        {
            
            _Updater.Add(_Center);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Updater.Shutdown();
        }

        void Regulus.Utility.IStage.Update()
        {
            _Updater.Update();

            foreach (var binder in _Binders.DequeueAll())
            {
                _Core.ObtainController(binder);
            }
        }
    }
}
