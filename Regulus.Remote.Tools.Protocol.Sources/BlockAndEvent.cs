using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Regulus.Remote.Tools.Protocol.Sources.BlockModifiers
{
    class BlockAndEvent
    {
        public BlockSyntax Block;
        public EventDeclarationSyntax Event;
    }
}