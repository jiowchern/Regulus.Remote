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
		public event Action<ICore, int, float> DoneEvent;

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
			_Command.Register<int, string, string>("Launch", _Start);
			_Command.Register<string>("LaunchIni", _StartIni);

			_View.WriteLine("======Ini file format description=====");
			_View.WriteLine("Example.");
			_View.WriteLine("[Launch]");
			_View.WriteLine("port = 12345");
			_View.WriteLine("path = game.dll");
			_View.WriteLine("class = Company.Project.Center");
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

		private void _StartIni(string path)
		{
			var ini = new Ini(File.ReadAllText(path));
			var port_string = ini.Read("Launch", "port");
			var port = int.Parse(port_string);
			var dllpath = ini.Read("Launch", "path");
			var className = ini.Read("Launch", "class");

			_Start(port, dllpath, className);
		}

		private void _Start(int port, string path, string class_name)
		{
			var stream = File.ReadAllBytes(path);

			try
			{
                var assembly = Assembly.Load(stream);
                var instance = assembly.CreateInstance(class_name);
			    var asm = assembly.GetName();

                _View.WriteLine($"Version : {asm.Version.ToString()}");             
                Log.Instance.WriteInfo($"Assembly Version : {asm.Version.ToString()}");
                DoneEvent(instance as ICore, port, 0);
			}
			catch(SystemException ex)
			{
				_View.WriteLine(ex.ToString());
			}
		}
	}
}
