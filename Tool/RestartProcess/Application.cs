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

                Viewer.WriteLine($"Parse path [{path}]");
                Viewer.WriteLine($"Parse args [{args}]");
                Viewer.WriteLine($"Parse WorkingDirectory [{file.DirectoryName}]");

                var process = Process.Start(info);
                process.EnableRaisingEvents = true;                
                process.Exited += (object sender, EventArgs e) => { _Restart(command); };

                
                
                _Restarts.Add(command);
                Viewer.WriteLine($"Launch process [{process.Id}] ");
            }
            catch(Exception e)
            {
                Viewer.WriteLine($"Launch fail. {e.ToString()}");                
            }
            
        }

        private void _Error(object sender, DataReceivedEventArgs e)
        {
            Viewer.WriteLine($"error tag.");
        }

        private void _LauchError(string command)
        {
            Viewer.WriteLine($"Launch fail. {command}");
        }

        private void _Restart(string path)
        {
            if(_Restarts.Any(p => p == path))
            {
                Viewer.WriteLine($"5 second restart. {path}");
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
