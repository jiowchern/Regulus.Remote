using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CSharp;

namespace Regulus.Remote.Unity
{
    public class CodeOutputer
    {        

        private readonly Assembly _CommonAsm;

        private readonly string _AgentName;

        private readonly Type[] _Types;

        private string _AgentNamespace;

        private readonly string _ProtocolName;

        public CodeOutputer(Assembly common_asm, string agent_name)
        {
            _CommonAsm = common_asm;
            
            _AgentName = agent_name;

            _Types = _GetTypes(common_asm).ToArray();

            var tokens = agent_name.Split(new[] { '.' });
            _AgentNamespace = string.Join(".", tokens.Take(tokens.Count() - 1).ToArray());

            _ProtocolName = _AgentNamespace + ".Protocol";
        }

       

        
        private IEnumerable<Type> _GetTypes(Assembly asm)
        {
            var types = asm.GetExportedTypes();
            foreach (var type in types)
            {
                yield return type;
            }
        }

        

        public void Output(string output_path)
        {

            if(System.IO.Directory.Exists(output_path))
                System.IO.Directory.Delete(output_path , true);
            var proxy = output_path + "/Proxy";
            var adsorber = output_path + "/Adsorber";
            var broadcaster = output_path + "/Broadcaster";
            System.IO.Directory.CreateDirectory(proxy);
            System.IO.Directory.CreateDirectory(adsorber);
            System.IO.Directory.CreateDirectory(broadcaster);
            

            var unityProtocolBuilder = new Regulus.Remote.Unity.CodeBuilder(_Types, _AgentName , _ProtocolName);

            unityProtocolBuilder.AgentEvent += (name,code) => _WriteFile(_GetFile(output_path, name), code);
            unityProtocolBuilder.TypeEvent += (name, ads, b) =>
            {
                _WriteFile(_GetFile(adsorber, name + "Adsorber"), ads);
                _WriteFile(_GetFile(broadcaster, name + "Broadcaster"), b);
                
            };
            unityProtocolBuilder.Build();
        }

        private void _WriteFile(string path, string code)
        {
            System.IO.File.WriteAllText(path , code);
        }

        private string _GetFile(string output_path, string name)
        {
            return  string.Format("{0}\\{1}.cs" , output_path , name);
        }
    }
}