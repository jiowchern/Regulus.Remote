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
        [System.Flags]
        public enum LogFilter
        {
            None = 0,
            RegisterCommand = 1,
            All = RegisterCommand
        }

        public Command Command { get; private set; }
        IInput _Input;
        IViewer _Viewer;
        LogFilter _Filter;

		public Console(IInput input, IViewer viewer)
        {
            Command = new Command();
            _Filter = LogFilter.All;
			_Initial(input , viewer);
        }

		~Console()
		{
			_Release();
		}

        void _Initial(IInput input , IViewer viewer)
        {
            _Viewer = viewer;
            _Input = input;
            _Input.OutputEvent += _Run;
            
            Command.RegisterEvent += _OnRegister;
            Command.UnregisterEvent += _OnUnregister;
        }
       
        public void SetLogFilter(LogFilter flag)
        {
            _Filter = flag;
        }
        void _OnUnregister(string command)
        {
            if ( (_Filter & LogFilter.RegisterCommand) == LogFilter.RegisterCommand )
                _Viewer.WriteLine("Remove Command> " + command);
        }
        void _Release()
        {
            Command.UnregisterEvent -= _OnUnregister;
            Command.RegisterEvent -= _OnRegister;
            _Input.OutputEvent -= _Run;
        }

        void _OnRegister(string command, Command.CommandParameter ret, Command.CommandParameter[] args)
        {
            if ((_Filter & LogFilter.RegisterCommand) == LogFilter.RegisterCommand)
            {
                string argString = "";
                foreach (var arg in args)
                {

                    argString +=  (string.IsNullOrEmpty(arg.Description)? "": arg.Description + ":") + arg.Param.Name + " ";
                }
                _Viewer.WriteLine("Add Command> " + command + " " + argString);
            }            
        }

        void _Run(string[] command_paraments)
        {
            System.Collections.Generic.Queue<string> cmdArgs = new System.Collections.Generic.Queue<string>(command_paraments);            
            if (cmdArgs.Count > 0)
            {          
                var cmd = cmdArgs.Dequeue();

                try
                {
                    int runCount = Command.Run(cmd, cmdArgs.ToArray());
                    if (runCount != 0)
                    {
                        _Viewer.WriteLine("Done.");
                    }
                    else
                    {
                        _Viewer.WriteLine("Without this command.");
                    }
                }
                catch (System.ArgumentException argument_exception)
                {
                    _Viewer.WriteLine("Parameter error: " + argument_exception.Message);
                    
                }
                
            }
        }
    }
}