using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Immutable;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        void ISourceGenerator.Execute(GeneratorExecutionContext context)
        {
           

            var logger = new DialogProvider();

            try
            {
                var tag = _GetTag(context.AdditionalFiles, context.Compilation);

                var references = new EssentialReference(context.Compilation, tag);

                var psb = new ProjectSourceBuilder(references);
                
                var sources = psb.Sources;

                foreach (var item in logger.Unsupports(psb.ClassAndTypess))
                {
                    context.ReportDiagnostic(item);
                }


                foreach (var syntaxTree in sources)
                {
                    context.AddSource(syntaxTree.FilePath, syntaxTree.GetText());
                }
                context.ReportDiagnostic(logger.Done());
                
            }
            catch (MissingTypeException e)
            {
                context.ReportDiagnostic(logger.MissingReference(e));                
                
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(logger.Exception(e.ToString()));                
            }
            
            
        }

        private INamedTypeSymbol _GetTag(ImmutableArray<AdditionalText> additionalFiles ,Compilation com )
        {
            
            
            return additionalFiles.GetConfigurations().FirstOrDefault()?.GetTag(com);
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
