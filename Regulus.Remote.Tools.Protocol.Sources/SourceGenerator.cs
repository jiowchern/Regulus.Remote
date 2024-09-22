using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

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
                INamedTypeSymbol tag = new IdentifyFinder(context.Compilation).Tag;

                var references = new EssentialReference(context.Compilation, tag);

                var psb = new ProjectSourceBuilder(references);
                
                var sources = psb.Sources;

                foreach (var item in logger.Unsupports(psb.ClassAndTypess))
                {
                    context.ReportDiagnostic(item);
                }


                foreach (var syntaxTree in sources)
                {
#if DEBUG
                      // System.IO.File.WriteAllText(syntaxTree.FilePath, syntaxTree.GetText().ToString());
#endif
                    context.AddSource(syntaxTree.FilePath, syntaxTree.GetText());


                }


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
