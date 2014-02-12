using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeysUserConsole
{
    class CommandHandler
    {
        public CommandHandler()
        {            
            
        }

        class CommandInfomation
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public Action<string[]> Execute { get; set; }
        }
        List<CommandInfomation> _CommandInfomations = new List<CommandInfomation>();


        public void Help()
        {
            _PrintCommands(_CommandInfomations);
        }

        private void _PrintCommands(List<CommandInfomation> _CommandInfomations)
        {
            foreach (var ci in _CommandInfomations)
            {
                Console.WriteLine( ci.Name + "\t\t\t" + ci.Description );
            }
        }
        internal void Initialize()
        {
            Set("help", (args) => { Help(); }, "顯示命令列表.");            
        }

        public void Set(string command, Action<string[]> executer , string description)
        {
            
            _CommandInfomations.RemoveAll(cai => cai.Name.ToLower() == command.ToLower());
            _CommandInfomations.Add(new CommandInfomation() { Name = command, Execute = executer, Description = description });
            Console.WriteLine("新增命令 " + command);
        }

        public void Rise(string command)
        {

            _CommandInfomations.RemoveAll(cai => cai.Name.ToLower() == command.ToLower());
            Console.WriteLine("移除命令 " + command);
        }

        public void Run(string command , string[] args)
        {
            var commandInfomation = (from ci in _CommandInfomations where ci.Name.ToLower() == command.ToLower() select ci).FirstOrDefault();
            if (commandInfomation != null)
            {
                commandInfomation.Execute(args);                
            }
            else
            {
                Console.WriteLine("無法解析命令 "+command+ " .");
            }
            
        }
        internal void Finialize()
        {
            
        }
    }
}
