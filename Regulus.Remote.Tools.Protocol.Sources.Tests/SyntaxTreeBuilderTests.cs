using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{

    using NUnit.Framework;

    public class SyntaxTreeBuilderTests
    {
        [Test]
        public void InterfacesTest()
        {
            var source = @"public interface IA {}";
            var syntaxBuilder = new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source));
            var interfaces = syntaxBuilder.GetInterfaces("IA");
            Assert.AreEqual(1 , interfaces.Count());
        }
    }
}