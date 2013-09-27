namespace Regulus.Utility
{   
    public class Console
    {
        public delegate void OnOutput(string[] command_paraments );

        
        public interface IInput
        {
            event OnOutput OutputEvent;
        }
        public interface IViewer
        {
            void WriteLine(string message);
            void Write(string message);
        }
        public Command Command { get; private set; }
        IInput _Input;
        IViewer _Viewer;
        
        public Console()
        {
            Command = new Command();            
        }

        public void Initial(IInput input , IViewer viewer)
        {
            _Viewer = viewer;
            _Input = input;
            _Input.OutputEvent += _Run;
            
            Command.RegisterEvent += _OnRegister;
            Command.UnregisterEvent += _OnUnregister;
        }

        void _OnUnregister(string command)
        {
            _Viewer.WriteLine("移除命令" + command);
        }
        public void Release()
        {
            Command.UnregisterEvent -= _OnUnregister;
            Command.RegisterEvent -= _OnRegister;
            _Input.OutputEvent -= _Run;
        }

        void _OnRegister(string command, System.Type ret, System.Type[] args)
        {
            string argString = ""; 
            foreach (var arg in args)
            {
                argString += arg.Name + " ";
            }
            _Viewer.WriteLine("增加命令 " + command + " " + argString);
        }

        void _Run(string[] command_paraments)
        {
            System.Collections.Generic.Queue<string> cmdArgs = new System.Collections.Generic.Queue<string>(command_paraments);            
            if (cmdArgs.Count > 0)
            {          
                var cmd = cmdArgs.Dequeue();
                int runCount = Command.Run(cmd, cmdArgs.ToArray());
                if (runCount != 0)
                {
                    _Viewer.WriteLine("執行完畢.");
                }
                else
                {
                    _Viewer.WriteLine("無此命令.");
                }
            }
        }
    }
}