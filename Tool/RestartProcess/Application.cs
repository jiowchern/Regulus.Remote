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

        private void _Add(string path)
        {
            try
            {
                var process = Process.Start(path);
                process.EnableRaisingEvents = true;
                process.Exited += (object sender, EventArgs e) => { _Restart(path); };
                _Restarts.Add(path);
                Viewer.WriteLine($"Launch process [{process.Id}] {path}");
            }
            catch(Exception e)
            {
                Viewer.WriteLine($"Launch fail. {e.ToString()}");                
            }
            
        }

        private void _Restart(string path)
        {
            if(_Restarts.Any(p => p == path))
                _Add(path);
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
