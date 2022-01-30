using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    internal class TypeDecomposer
    {
        public readonly System.Collections.Generic.IEnumerable<string> Types;
        private readonly System.Collections.Generic.HashSet<string> _Types;
        public TypeDecomposer(ParameterSyntax node)
        {
            _Types = new HashSet<string>();
            Types = _Types;
            _Decompose(node);
        }

        private void _Decompose(ParameterSyntax node)
        {
            var kind = node.Type.Kind();
            _Types.Add(node.Type.ToFullString());
            if (kind != SyntaxKind.PredefinedType)
            {
                
            }
            
        }
    }
}