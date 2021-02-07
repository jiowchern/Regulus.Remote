using System.IO;
using System.Linq;

namespace Regulus.Application
{
    class TemplateCreator
    {
        private readonly DirectoryInfo _OutputDir;
        private readonly string _SlnNamespace;
        private readonly DirectoryInfo _LibraryDir;

        System.Threading.Tasks.Task<int> _RunDotnet(string args)
        {
            return _Run("dotnet" , args);
        }
        async System.Threading.Tasks.Task<int> _Run(string command , string args)
        {
            var info = new System.Diagnostics.ProcessStartInfo(command, args);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = info;
            proc.Start();
            proc.WaitForExit();
            return proc.ExitCode;
        }
        public TemplateCreator(System.IO.DirectoryInfo output_dir,string sln_namespace , System.IO.DirectoryInfo library_dir )
        {
            _OutputDir = output_dir;
            _SlnNamespace = sln_namespace;
            _LibraryDir = library_dir;
        }

        public async System.Threading.Tasks.Task<bool> Run()
        {

            // new sln
            var slnPath = System.IO.Path.GetFullPath(_OutputDir.FullName);
            var slnResult = await _RunDotnet($"new sln -o {slnPath}");
            if (slnResult != 0)
                return false;

            // create projects
            var commonNamespace = $"{_SlnNamespace}.Common";
            var mainNamespace = $"{_SlnNamespace}.Main";
            var protocolNamespace = $"{_SlnNamespace}.Protocol";
            var commonPath = System.IO.Path.GetFullPath(commonNamespace, _OutputDir.FullName);
            var mainPath = System.IO.Path.GetFullPath(mainNamespace, _OutputDir.FullName);
            var protocolPath = System.IO.Path.GetFullPath(protocolNamespace, _OutputDir.FullName);
            var createResult = await System.Threading.Tasks.Task.WhenAll(
                _RunDotnet($"new classlib -f netstandard2.0 -o {commonPath}"),
                _RunDotnet($"new classlib -f netstandard2.0 -o {mainPath}"),
                _RunDotnet($"new classlib -f netstandard2.0 -o {protocolPath}")
                );

            if (createResult.Any(r => r != 0))
                return false;


            // add regulus to sln 

            var commonProjPath = System.IO.Path.Combine(commonPath, $"{commonNamespace}.csproj");
            var mainProjPath = System.IO.Path.Combine(mainPath, $"{mainNamespace}.csproj");
            var protocolProjPath = System.IO.Path.Combine(protocolPath, $"{protocolNamespace}.csproj");
            
            var slnFilePath = System.IO.Path.Combine(slnPath , $"{_OutputDir.Name}.sln" );

            var regulusRemoteServerProj = System.IO.Path.Combine(_LibraryDir.FullName , "Regulus.Remote.Server/Regulus.Remote.Server.csproj");
            var regulusRemoteClientProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Remote.Client/Regulus.Remote.Client.csproj");
            var regulusRemoteStandaloneProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Remote.Standalone/Regulus.Remote.Standalone.csproj");
            
            var regulusNetworkProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Network/Regulus.Network.csproj");
            var regulusRemoteProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Remote/Regulus.Remote.csproj");
            var regulusRemoteProtocolProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Remote.Protocol/Regulus.Remote.Protocol.csproj");
            var regulusRemoteSoulProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Remote.Soul/Regulus.Remote.Soul.csproj");
            var regulusRemoteGhostProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Remote.Ghost/Regulus.Remote.Ghost.csproj");
            var regulusSerializationProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Serialization/Regulus.Serialization.csproj");
            var regulusUtilityProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Utility/Regulus.Utility.csproj");
            var addProjectToSlnResult = await System.Threading.Tasks.Task.WhenAll(
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteServerProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteClientProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteStandaloneProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusNetworkProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteProtocolProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteSoulProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusRemoteGhostProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusUtilityProj}"),
                _RunDotnet($"sln {slnFilePath} add -s Regulus.Library {regulusSerializationProj}"),
                _RunDotnet($"sln {slnFilePath} add {commonProjPath}"),
                _RunDotnet($"sln {slnFilePath} add {mainProjPath}"),
                _RunDotnet($"sln {slnFilePath} add {protocolProjPath}")
                );
            if (addProjectToSlnResult.Any(r => r != 0))
                return false;

            // ref project to project

            var refResult = await System.Threading.Tasks.Task.WhenAll(
                _RunDotnet($"add {mainProjPath} reference {commonProjPath}"),
                _RunDotnet($"add {protocolProjPath} reference {commonProjPath}"),

                _RunDotnet($"add {commonProjPath} reference {regulusRemoteProj}"),                
                _RunDotnet($"add {mainProjPath} reference {regulusRemoteProj}"),
                _RunDotnet($"add {protocolProjPath} reference {regulusRemoteProj}"),
                _RunDotnet($"add {protocolProjPath} reference {regulusSerializationProj}")
                );
            if (refResult.Any(r => r != 0))
                return false;

            // 

            var regulusApplicationProtocolCodeWriterProj = System.IO.Path.Combine(_LibraryDir.FullName, "Regulus.Application.Protocol.CodeWriter");
            string t = @$"
<Project>
    <Target Condition=""'$(ProjectName)' == '{commonNamespace}' And '$(SolutionDir)' != '*Undefined*'"" Name=""CreateProtocol"" AfterTargets=""Build"">
            <Exec Command = ""del {protocolPath}\*.cs /q"" />     
             <Exec Command = ""dotnet run --project {regulusApplicationProtocolCodeWriterProj}  --common $(TargetPath) --output {protocolPath}"" />      
          </Target>
      </Project>
      ";
            System.IO.File.WriteAllText(System.IO.Path.Combine(slnPath, "Directory.Build.targets"), t);
            return true;

        }
    }
}
