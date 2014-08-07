using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Soul.Native
{
	partial class Controller : Application.IController
	{
        Regulus.Game.StageMachine _Machine;

        
		Regulus.Utility.Command _Command;
		Regulus.Utility.Console.IViewer _View;
		

		public Controller(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view)
		{
			
			_Command = command;
			_View = view;
		
            _Machine = new Game.StageMachine();
		}


		string _Name;
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}
		

		public void Look()
		{
			
			
		}

		public void NotLook()
		{            
			
			
		}
		
		public bool Update()
		{            
            _Machine.Update();						
			
			return true;
		}

		public void Launch()
		{
            _ToStart();            
		}

        private void _ToStart()
        {
            var stage = new Regulus.Remoting.Soul.Native.Controller.StageStart(_Command,_View);
            stage.DoneEvent += _ToRun;
            _Machine.Push(stage);
        }
        private void _ToRun(Regulus.Game.ICore core,int port , float timeout)
        {
            var stage = new Regulus.Remoting.Soul.Native.Controller.StageRun(core, _Command, port , _View);
            stage.ShutdownEvent += _ToStart;            
            _Machine.Push(stage);
        }
        
		public void Shutdown()
		{
            _Machine.Termination();
		}

        string Game.ConsoleFramework<IUser>.IController.Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void Game.ConsoleFramework<IUser>.IController.Look()
        {
            throw new NotImplementedException();
        }

        void Game.ConsoleFramework<IUser>.IController.NotLook()
        {
            throw new NotImplementedException();
        }

        IUser Game.ConsoleFramework<IUser>.IController.GetUser()
        {
            return null;
        }

        bool Utility.IUpdatable.Update()
        {
            throw new NotImplementedException();
        }

        void Framework.ILaunched.Launch()
        {
            throw new NotImplementedException();
        }

        void Framework.ILaunched.Shutdown()
        {
            throw new NotImplementedException();
        }
    }

	
}
