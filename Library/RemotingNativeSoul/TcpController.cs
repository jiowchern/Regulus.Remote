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
		Regulus.Utility.FPSCounter _FPS;

		public TcpController(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view)
		{
			
			_Command = command;
			_View = view;
			_FPS = new Utility.FPSCounter();
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
			
			_Command.Register("FPS", () => { _View.WriteLine("FPS:" + _FPS.Value.ToString()); });
		}

		public void NotLook()
		{            
			
			_Command.Unregister("FPS");
		}
		
		public bool Update()
		{
            System.Threading.Thread.Sleep(0);
            _Machine.Update();						

			_FPS.Update();            
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
			
		}
        
        

		
	}

	
}
