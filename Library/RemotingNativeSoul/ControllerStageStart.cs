// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerStageStart.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StageStart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.IO;

using Regulus.Utility;

using Console = Regulus.Utility.Console;

#endregion

namespace Regulus.Remoting.Soul.Native
{
	internal class StageStart : IStage
	{
		public event Action<ICore, int, float> DoneEvent;

		private readonly Command _Command;

		private readonly Console.IViewer _View;

		public StageStart(Command command, Console.IViewer view)
		{
			this._View = view;
			this._Command = command;
		}

		void IStage.Enter()
		{
			this._Command.Register<int, string, string>("Launch", this._Start);
			this._Command.Register<string>("LaunchIni", this._StartIni);

			this._View.WriteLine("======Ini file format description=====");
			this._View.WriteLine("Example.");
			this._View.WriteLine("[Launch]");
			this._View.WriteLine("port = 12345");
			this._View.WriteLine("path = game.dll");
			this._View.WriteLine("class = Company.Project.Center");
			this._View.WriteLine("======================================");
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
			this._Command.Unregister("Launch");
			this._Command.Unregister("LaunchIni");
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

			this._Start(port, dllpath, className);
		}

		private void _Start(int port, string path, string class_name)
		{
			var stream = File.ReadAllBytes(path);

			try
			{
				var core = Loader.Load(stream, class_name);
				this.DoneEvent(core, port, 0);
			}
			catch (SystemException ex)
			{
				this._View.WriteLine(ex.ToString());
			}
		}
	}
}