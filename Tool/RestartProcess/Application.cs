using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


using Regulus.Utility;

namespace Regulus.Project.RestartProcess
{
    internal class Application : Regulus.Utility.WindowConsole
    {
        private readonly HashSet<string> _Restarts;

        public Application()
        {
            _Restarts = new HashSet<string>();
        }

        public Application(string paths_path) : this()
        {
            if(System.IO.File.Exists(paths_path))
            {
                var paths = System.IO.File.ReadAllLines(paths_path);
                foreach (var path in paths)
                {
                    _Add(path);
                }
            }
            
        }

        protected override void _Launch()
        {
            Command.Register<string>("Add[path]" , _Add);
            Command.Register<string>("Del[path]", _Del);
        }

        private void _Del(string path)
        {
            _Restarts.Remove(path);            
        }

        private void _Add(string command)
        {
            try
            {
                
                var result = command.Split(' ');
                var path = result[0];
                var args = string.Join(" ", result.Skip(1).ToArray());

                

                var file = new System.IO.FileInfo(path);
                if(file.Exists == false)
                {
                    _LauchError(command);
                    return;
                }
                var info = new ProcessStartInfo();                
                
                info.FileName = path;
                info.Arguments = args;
                info.WorkingDirectory = file.DirectoryName;

                Log.Instance.WriteInfo($"Parse path [{path}]");
                Log.Instance.WriteInfo($"Parse args [{args}]");
                Log.Instance.WriteInfo($"Parse WorkingDirectory [{file.DirectoryName}]");

                var process = Process.Start(info);
                process.EnableRaisingEvents = true;                
                process.Exited += (object sender, EventArgs e) => { _Restart(command); };

                
                
                _Restarts.Add(command);
                Log.Instance.WriteInfo($"Launch process [{process.Id}] ");
            }
            catch(Exception e)
            {
                Log.Instance.WriteInfo($"Launch fail. {e.ToString()}");                
            }
            
        }

       

        private void _LauchError(string command)
        {
            Log.Instance.WriteInfo($"Launch fail. {command}");
        }

        private void _Restart(string path)
        {
            if(_Restarts.Any(p => p == path))
            {

                Log.Instance.WriteInfo($"5 second restart. {path}");
                var timer = new System.Timers.Timer();
                timer.Interval = 5000f;                
                timer.Enabled = true;
                timer.AutoReset = false;
                timer.Elapsed += (sender, args) => { _Add(path); };
                timer.Start();                
            }
                
        }

        protected override void _Update()
        {
            
        }

        protected override void _Shutdown()
        {
            Command.Unregister("Add");
            Command.Unregister("Del");
        }
    }
}
