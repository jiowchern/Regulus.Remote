using System;
using System.IO;
using System.Linq;
using System.Reflection;


using Regulus.Utility;


using Console = Regulus.Utility.Console;

namespace Regulus.Remoting.Soul.Native
{
	internal class StageStart : IStage
	{
		public event Action<ICore,IProtocol, int, float> DoneEvent;

		private readonly Command _Command;

		private readonly Console.IViewer _View;

		private readonly string[] _FirstCommand;

		public StageStart(Command command, Console.IViewer view ,string[] first_command)
		{
			_View = view;
			_FirstCommand = first_command;
			_Command = command;
		}

		void IStage.Enter()
		{
						
			_Command.RegisterLambda<StageStart, string>(this , (instance, ini_path ) => instance.LaunchIni(ini_path));
			

			_View.WriteLine("======Ini file format description=====");
			_View.WriteLine("Example.");
			_View.WriteLine("[Launch]");
			_View.WriteLine("port = 12345");
			_View.WriteLine("game = game.dll");
            _View.WriteLine("game_entry = Company.Project.Center");
            _View.WriteLine("protocol = protocol.dll");
            _View.WriteLine("protocol_entry = Company.Project.Protocol");

            _View.WriteLine("======================================");


			if(_HasFirstCommand())
			{
				_RunFirstCommand();
			}
			
		}

		private void _RunFirstCommand()
		{
			var command = _FirstCommand[0];
			var args = _FirstCommand.Skip(1).ToArray();
			var arg = string.Join(" ", args);
			_View.WriteLine(string.Format("First Run Command {0} {1}.", command, arg));
			_Command.Run(
				command,
				args);
			
		}

		private bool _HasFirstCommand()
		{
			return _FirstCommand.Length > 0;
		}

		/*private void _LoadLibrary(string work_dir)
		{
				
			var files = from f in System.IO.Directory.EnumerateFiles(work_dir, "*.dll", System.IO.SearchOption.AllDirectories) select f;
			foreach(var file in files)
			{
				_View.Write("載入程式庫 " + file + "...");
				try
				{
					var assembly = System.Reflection.Assembly.LoadFile(file);                        
					_View.WriteLine("完成!");
				}
				catch (SystemException ex)
				{
					_View.WriteLine("失敗!" + ex.ToString());
				}
					
			}
		}*/
		void IStage.Leave()
		{
			_Command.Unregister("Launch");
			_Command.Unregister("LaunchIni");
		}

		void IStage.Update()
		{
		}

		private void LaunchIni(string path)
		{
			var ini = new Ini(File.ReadAllText(path));
			var port_string = ini.Read("Launch", "port");
			var port = int.Parse(port_string);
			var dllpath = ini.Read("Launch", "game");
            var gpipath = ini.Read("Launch", "protocol");
            var className = ini.Read("Launch", "game_entry");
            var libraryName = ini.Read("Launch", "protocol_entry");

            Launch(port, dllpath , gpipath, className, libraryName);
		}

		public void Launch(int port, string game_path, string gpi_path, string class_name , string library_name)
		{
			

			try
			{
			    var core = _LoadGame(game_path, class_name);
                var library = _LoadLibrary(gpi_path, library_name);
                
				DoneEvent(core, library, port, 0);
			}
			catch(Exception ex)
			{
				_View.WriteLine(ex.ToString());
			}
		}

	    private IProtocol _LoadLibrary(string gpi_path , string library_entry)
	    {
            var stream = File.ReadAllBytes(gpi_path);
            var assembly = Assembly.Load(stream);
            var instance = assembly.CreateInstance(library_entry);
            var asm = assembly.GetName();

            _View.WriteLine($"Protocol Version : {asm.Version}");
            Log.Instance.WriteInfo($"Protocol Assembly Version : {asm.Version.ToString()}");

            return instance as IProtocol;
        }

	    private ICore _LoadGame(string game_path,string entry_name)
	    {
            var stream = File.ReadAllBytes(game_path);
            var assembly = Assembly.Load(stream);
            var instance = assembly.CreateInstance(entry_name);
            var asm = assembly.GetName();

            _View.WriteLine($"Game Version : {asm.Version}");
            Log.Instance.WriteInfo($"Game Assembly Version : {asm.Version.ToString()}");

	        return instance as ICore;
	    }
	}
}

