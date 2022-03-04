using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        void ISourceGenerator.Execute(GeneratorExecutionContext context)
        {

            var references = new EssentialReference(context.Compilation);

            var sources = new ProjectSourceBuilder(references).Sources;
            
            foreach (var syntaxTree in sources)
            {
                context.AddSource(syntaxTree.FilePath, syntaxTree.GetText());
            }                       
        }

        

        void ISourceGenerator.Initialize(GeneratorInitializationContext context)
        {

 #if DEBUG
             if (!Debugger.IsAttached)
             {
                 //Debugger.Launch();
             }
 #endif

        }
    }
}
