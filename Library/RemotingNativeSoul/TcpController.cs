using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Soul.Native
{
	partial class TcpController : Application.IController
	{
        Regulus.Game.StageMachine _Machine;

        
		Regulus.Utility.Command _Command;
		Regulus.Utility.Console.IViewer _View;
		

		public TcpController(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view)
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

		public event Game.ConsoleFramework<IUser>.OnSpawnUser UserSpawnEvent;

		public event Game.ConsoleFramework<IUser>.OnSpawnUserFail UserSpawnFailEvent;

		public event Game.ConsoleFramework<IUser>.OnUnspawnUser UserUnpawnEvent;

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
            var stage = new Regulus.Remoting.Soul.Native.TcpController.StageStart(_Command,_View);
            stage.DoneEvent += _ToRun;
            _Machine.Push(stage);
        }
        private void _ToRun(Regulus.Game.ICore core,int port , float timeout)
        {
            var stage = new Regulus.Remoting.Soul.Native.TcpController.StageRun(core, _Command, port , _View);
            stage.ShutdownEvent += _ToStart;            
            _Machine.Push(stage);
        }
        
		public void Shutdown()
		{
            _Machine.Termination();
		}
        
        

		
	}

	
}
