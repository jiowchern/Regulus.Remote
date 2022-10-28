using NUnit.Framework;
using NSubstitute;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class ConfigurationTests
    {
        [Test]
        public void Valid()
        {
            var text = NSubstitute.Substitute.For<Microsoft.CodeAnalysis.AdditionalText>();
            text.Path.Returns(Regulus.Remote.Tools.Protocol.Sources.Configuration.Filename);
            var cfg = text.GetConfiguration();
            NUnit.Framework.Assert.NotNull(cfg);
        }

        [Test]
        public void GetTag()
        {
            var text = NSubstitute.Substitute.For<Microsoft.CodeAnalysis.AdditionalText>();
            text.Path.Returns(Regulus.Remote.Tools.Protocol.Sources.Configuration.Filename);
            text.GetText().Returns(Microsoft.CodeAnalysis.Text.SourceText.From(@"
[Identification]
Tag = 1

"));
            var cfg = text.GetConfiguration();

            NUnit.Framework.Assert.AreEqual("1" , cfg.Identification.Tag);            
        }

        [Test]
        public void GetTagSynbol()
        {
            var text = NSubstitute.Substitute.For<Microsoft.CodeAnalysis.AdditionalText>();
            text.Path.Returns(Regulus.Remote.Tools.Protocol.Sources.Configuration.Filename);
            text.GetText().Returns(Microsoft.CodeAnalysis.Text.SourceText.From(@"
[Identification]
Tag = ""ITabable""
"));

            var source = @"

public interface ITabable {    
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var cfg = text.GetConfiguration();
            var symbol = cfg.GetTag(com);
            NUnit.Framework.Assert.NotNull(symbol);
        }

        [Test]
        public void GetTagSynboWithlNamespace()
        {
            var text = NSubstitute.Substitute.For<Microsoft.CodeAnalysis.AdditionalText>();
            text.Path.Returns(Regulus.Remote.Tools.Protocol.Sources.Configuration.Filename);
            text.GetText().Returns(Microsoft.CodeAnalysis.Text.SourceText.From(@"
[Identification]
Tag = ""NS.ITabable""
"));

            var source = @"
namespace NS
{
    public interface ITabable {    
    }
}

";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var cfg = text.GetConfiguration();
            var symbol = cfg.GetTag(com);
            NUnit.Framework.Assert.NotNull(symbol);
        }
    }
}