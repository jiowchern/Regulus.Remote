using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class ProtocolBuilderTests
    {


        [Test]
        public async Task ProtocolTest()
        {
            var source = @"
public interface IA
{
    
}
namespace NS1
{
    
    public class C1{}
    public interface IB
    {
       
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);


            await new GhostTest(new Configuration(), tree).RunAsync();
        }

    }


}