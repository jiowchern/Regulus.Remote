using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{
    partial class TcpController : Application.IController
    {
        class StageStart : Regulus.Game.IStage
        {
            public event Action<Regulus.Game.ICore> DoneEvent;
            TcpController _Controller;
            Regulus.Utility.Command _Command;
            public StageStart(Regulus.Utility.Command command , TcpController controller)
            {
                _Controller = controller;
                _Command = command;
            }
            void Game.IStage.Enter()
            {
                _Command.Register<int, float,string , string>("Launch", _Start );
                _Command.Register<string>("LaunchIni", _StartIni);
            }

            private void _StartIni(string path)
            {
                var ini = new Regulus.Utility.Ini(path);
                int port = int.Parse(ini.Read("Launch", "port"));

                float timeout = float.Parse(ini.Read("Launch", "timeout"));
                string dllpath = ini.Read("Launch", "path");
                string className = ini.Read("Launch", "class");

                _Start(port, timeout, dllpath, className);
            }

            private void _Start(int port, float timeout, string path, string class_name)
            {
                _Controller._StartListen(port, timeout);
                var stream = System.IO.File.ReadAllBytes(path);
                var core = Regulus.Game.Loader.Load(stream, class_name);                                
                DoneEvent(core);
            }

            void Game.IStage.Leave()
            {
                _Command.Unregister("Launch");
                _Command.Unregister("LaunchIni");
            }

            void Game.IStage.Update()
            {
                
            }
        }
    }
}
