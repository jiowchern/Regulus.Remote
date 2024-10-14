using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class DialogProvider
    {
        


        public static readonly DiagnosticDescriptor ExceptionDescriptor = new DiagnosticDescriptor("RRSE1", "Error", "unknown error:{0}", "Execute", DiagnosticSeverity.Error, false, null, "https://github.com/jiowchern/Regulus.Remote/wiki/RRSE1");
        
        public static readonly DiagnosticDescriptor UnsupportDescriptor = new DiagnosticDescriptor("RRSW1", "Warning", "Unsupport({0}):{1}", "Execute", DiagnosticSeverity.Warning, false, null, "https://github.com/jiowchern/Regulus.Remote/wiki/RRSW1");
        public static readonly DiagnosticDescriptor MissingReferenceDescriptor = new DiagnosticDescriptor("RRSW2", "Warning", "Missing essentialt type :{0}", "Execute", DiagnosticSeverity.Warning, false, null, "https://github.com/jiowchern/Regulus.Remote/wiki/RRSW2");
        public DialogProvider()
        {            
            
            
            
            
        }

        internal System.Collections.Generic.IEnumerable<Diagnostic> Unsupports(IEnumerable<ClassAndTypes> classAndTypess)
        {
            foreach (var cnt in classAndTypess)
            {
                var methods = cnt.GetSyntaxs<MethodDeclarationSyntax>().ToArray();
                var indexs = cnt.GetSyntaxs<IndexerDeclarationSyntax>().ToArray();
                var events = cnt.GetSyntaxs<EventDeclarationSyntax>().ToArray();
                var propertys = cnt.GetSyntaxs<PropertyDeclarationSyntax>().ToArray();                

                foreach (var item in indexs)
                {
                    yield return _Unsupport(item.WithAccessorList(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorList()) , "index" ) ;
                }
                foreach (var item in methods)
                {
                    yield return _Unsupport(item.WithBody(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Block()), "method");
                }
                foreach (var item in events)
                {
                    yield return _Unsupport(item.WithAccessorList(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorList()), "event");
                }
                foreach (var item in propertys)
                {
                    yield return _Unsupport(item.WithAccessorList(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorList()), "property");
                }
                

            }
            
        }

        private Diagnostic _Unsupport(SyntaxNode node, string type)
        {
            return Diagnostic.Create(UnsupportDescriptor, Location.None, type , node.NormalizeWhitespace().ToFullString());
        }

        public Diagnostic Exception(string msg)
        {
            return  Diagnostic.Create(ExceptionDescriptor, Location.None, msg);
            
        }

        internal Diagnostic MissingReference(MissingTypeException e)
        {
            return  Diagnostic.Create(MissingReferenceDescriptor, Location.None, e.ToString());            
        }

        
    }
}
