using Regulus.Remoting;
using Regulus.Remoting.Extension;

namespace Regulus.Tool
{
    internal class Application : Regulus.Utility.WindowConsole
    {
        private readonly string _Path;

        private readonly GhostProviderGenerator _GhostProviderGenerator;

        public Application(string path) : this
            ()
        {
            _Path = path;            
        }

        public Application()
        {
            _GhostProviderGenerator = new GhostProviderGenerator();
        }

        protected override void _Launch()
        {
            if (string.IsNullOrWhiteSpace(_Path) == false)
            {
                _Build(_Path);
            }
            Command.Register<string>("Build" , _Build);
        }

        private void _Build(string path)
        {
            
            var ini = new Regulus.Utility.Ini(System.IO.File.ReadAllText(path));

            var sourcePath = ini.Read("Setting", "SourcePath");            
            var sourceNamespace = ini.Read("Setting", "Namespace");
            var providerName = ini.Read("Setting", "ProviderName");
            var outputPath = ini.Read("Setting", "OutputPath");
            _GhostProviderGenerator.Build(sourcePath , outputPath, providerName, new string[] { sourceNamespace });

            
        }

        protected override void _Update()
        {
            
        }

        protected override void _Shutdown()
        {
            
        }
    }
}