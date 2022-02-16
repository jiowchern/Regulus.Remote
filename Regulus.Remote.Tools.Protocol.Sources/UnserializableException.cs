using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    namespace Exceptions
    {
        public class UnserializableException : System.Exception
        {
            public UnserializableException(ISymbol symbol) : base($"Unserializable type {symbol}.")
            {
            }
        }

    }
}