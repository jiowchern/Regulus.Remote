using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


using Regulus.Remote.Tools.Protocol.Sources.Extensions;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    class TypeSyntaxEqualityComparer : IEqualityComparer<TypeSyntax>
    {
        bool IEqualityComparer<TypeSyntax>.Equals(TypeSyntax x, TypeSyntax y)
        {
            var b = x.IsEquivalentTo(y);
            return b;
        }

        int IEqualityComparer<TypeSyntax>.GetHashCode(TypeSyntax obj)
        {
            
            return obj.ToFullString().GetHashCode();
        }
    }

    public class GhostBuilder  
    {
        public readonly IEnumerable<TypeSyntax> Types;
        public readonly IEnumerable<ClassDeclarationSyntax> Ghosts;
        public readonly IEnumerable<ClassDeclarationSyntax> EventProxys;


        public GhostBuilder(IEnumerable<INamedTypeSymbol> symbols)
        {
            
            var builders = new Dictionary<INamedTypeSymbol, InterfaceInheritor>(SymbolEqualityComparer.Default);
            foreach (var item in from i in symbols
                                 select new KeyValuePair<INamedTypeSymbol, InterfaceInheritor>(i, new InterfaceInheritor(i.ToInferredInterface())))
            {
                if(!builders.ContainsKey(item.Key))
                    builders.Add(item.Key, item.Value);
            }


            var types  = new System.Collections.Generic.List<TypeSyntax>();
            var ghosts = new System.Collections.Generic.List<ClassDeclarationSyntax>();
            var eventProxys= new System.Collections.Generic.List<ClassDeclarationSyntax>();
            foreach (var symbol in symbols)
            {
                var name = $"C{symbol.ToDisplayString().Replace('.', '_')}";
                var type = SyntaxFactory.ClassDeclaration(name);

                foreach (var i2 in symbol.AllInterfaces.Union(new[] { symbol }))
                {

                    var builder = builders[i2];
                    type = builder.Inherite(type);
                }

                eventProxys.AddRange(type.DescendantNodes().OfType<EventDeclarationSyntax>().Select(e => e.CreateRegulusRemoteIEventProxyCreater()));

                var modifier = new SyntaxModifier(type);
                types.AddRange(modifier.TypesOfSerialization);
                type = modifier.Type;                
                type = type.ImplementRegulusRemoteIGhost();                
                ghosts.Add(type);
            }

            
            Types = new HashSet<TypeSyntax>(types , new TypeSyntaxEqualityComparer());
            Ghosts = new HashSet<ClassDeclarationSyntax>(ghosts, new ClassDeclarationSyntaxEqualityComparer());
            EventProxys = new HashSet<ClassDeclarationSyntax>(eventProxys, new ClassDeclarationSyntaxEqualityComparer()); 
        }
    }

}