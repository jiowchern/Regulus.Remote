using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
namespace Regulus.Remote.Tools.Protocol.Sources.BlockModifiers
{
    internal class PropertyRegulusRemoteBlock
    {
        private readonly Compilation _Compilation;

        public PropertyRegulusRemoteBlock(Compilation compilation)
        {
            this._Compilation = compilation;
        }
        public PropertyAndBlock Mod(System.Collections.Generic.IEnumerable<SyntaxNode> nodes)
        {
            var block = nodes.Skip(0).FirstOrDefault() as BlockSyntax;
            var ad = nodes.Skip(1).FirstOrDefault() as AccessorDeclarationSyntax;
            var al = nodes.Skip(2).FirstOrDefault() as AccessorListSyntax;
            var pd = nodes.Skip(3).FirstOrDefault() as PropertyDeclarationSyntax;
            var cd = nodes.Skip(4).FirstOrDefault() as ClassDeclarationSyntax;

            if (Extensions.SyntaxExtensions.AnyNull(block, ad , al,pd, cd))
            {
                return null;
            }

            var qn = pd.Type as QualifiedNameSyntax;

            if (qn == null)
                return null;

            if (qn.Left.ToString() != "Regulus.Remote")
                return null;

            var gn = qn.Right as GenericNameSyntax;
            if (gn == null)
                return null;

            if (gn.Identifier.ToString() != "Property" && gn.Identifier.ToString() != "Notifier")
                return null;

            if(gn.Identifier.ToString() == "Property")
            {
                if (!_Compilation.AllSerializable(gn.TypeArgumentList.Arguments))
                {
                    return null;
                }
            }

            if (gn.Identifier.ToString() == "Notifier")
            {
                if(!_Compilation.AllGhostable(gn.TypeArgumentList.Arguments))
                {
                    return null;
                }                
            }


            if (!ad.IsKind(SyntaxKind.GetAccessorDeclaration))
            {
                return null;
            }

            var ownerName = pd.ExplicitInterfaceSpecifier.Name;
            var name = $"_{ownerName}.{pd.Identifier}";
            name = name.Replace('.', '_');

            var newBlock = SyntaxFactory.Block(SyntaxFactory.ParseStatement(
$@"
return {name};
"));
            return new PropertyAndBlock() { Block =newBlock  , Property = pd };
        }
    }
}