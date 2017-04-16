using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


using Regulus.Utility;


using Console = Regulus.Utility.Console;

namespace Regulus.Remoting.Soul.Native
{
	internal class StageStart : IStage
	{
		public event Action<ICore,IProtocol, int> DoneEvent;

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
			_View.WriteLine("project_path = path/project.dll");
			_View.WriteLine("project_entry = YourNamespace.YourProjectClassName"); 
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
			_Command.Run(command,args);	 
		}

		private bool _HasFirstCommand()
		{
			return _FirstCommand.Length > 0;
		}

		
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
            try
            {
                var ini = new Ini(File.ReadAllText(path));
			    var port_string = ini.Read("Launch", "port");
			    var port = int.Parse(port_string);
			    var dllpath = ini.Read("Launch", "project_path");			
			    var className = ini.Read("Launch", "project_entry");
			

			    Launch(port, dllpath , className);
            }
            catch (Exception ex)
            {
                _View.WriteLine(ex.ToString());
            }
        }

		public void Launch(int port, string gpi_path, string entry_name)
		{

            var stream = File.ReadAllBytes(gpi_path);
            var assembly = Assembly.Load(stream);
            var instance = assembly.CreateInstance(entry_name) as ICore;
            var asm = assembly.GetName();

            _View.WriteLine($"Project Version : {asm.Version}");
            Log.Instance.WriteInfo($"Project Version : {asm.Version}");
                
            var library = _LoadProtocol(assembly , entry_name);
				
			DoneEvent(instance, library, port);			
		}

		private IProtocol _LoadProtocol(Assembly gpi , string entry_name)
		{

            var nameSpaces = new HashSet<string>();
		    foreach (var type in gpi.GetExportedTypes())
		    {
		        nameSpaces.Add(type.Namespace);
		    }
            
            var protocolName = entry_name + "ProtocolProvider";
            var buidler = new Regulus.Protocol.AssemblyBuilder();
		    var asm =  buidler.Build(gpi,protocolName, nameSpaces.ToArray());
		    return asm.CreateInstance(protocolName) as IProtocol;
		}

		
	}
}

