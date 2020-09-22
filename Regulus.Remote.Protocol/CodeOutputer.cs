using System.Text;

namespace Regulus.Remote.Protocol
{
    public class CodeOutputer
    {
        private readonly System.Reflection.Assembly _CommonAssembly;


        public CodeOutputer(System.Reflection.Assembly common_assembly)
        {
            _CommonAssembly = common_assembly;
        }

        public void Output(string out_path)
        {
            _Output(out_path, _CommonAssembly);
        }
        private static void _Output(string outPath, System.Reflection.Assembly inputAssembly)
        {
            CodeBuilder builder = new Regulus.Remote.Protocol.CodeBuilder();
            builder.ProviderEvent += (name, code) => _WriteProvider(name, code, outPath);
            builder.EventEvent += (type_name, event_name, code) => _WriteEvent(type_name, event_name, code, outPath);
            builder.GpiEvent += (type_name, code) => _WriteType(type_name, code, outPath);
            builder.Build(inputAssembly);
        }

        private static void _WriteType(string type_name, string code, string outPath)
        {
            var path = System.IO.Path.Combine(outPath, $"{type_name}.cs");
            Regulus.Utility.Log.Instance.WriteInfo(path);

            System.IO.File.WriteAllText(path, code, Encoding.ASCII);
        }

        private static void _WriteEvent(string type_name, string event_name, string code, string outPath)
        {
            var path = System.IO.Path.Combine(outPath, $"{type_name}_{event_name}.cs");
            Regulus.Utility.Log.Instance.WriteInfo(path);

            System.IO.File.WriteAllText(path, code, Encoding.ASCII);
        }

        private static void _WriteProvider(string name, string code, string outPath)
        {
            var path = System.IO.Path.Combine(outPath, $"{name}.cs"); 
            Regulus.Utility.Log.Instance.WriteInfo(path);

            
            System.IO.File.WriteAllText(path, code, Encoding.ASCII);
        }
    }
}
