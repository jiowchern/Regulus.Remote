using Regulus.Remote.Extensions;
using NSubstitute;
namespace RemotingTest
{
    public class ExpressionsExtensionsTests
    {
        [NUnit.Framework.Test]
        public void Test()
        {
            var accesser = NSubstitute.Substitute.For<Regulus.Remote.IObjectAccessible>();

            System.Linq.Expressions.Expression<Regulus.Remote.GetObjectAccesserMethod> exp = (a) => a.Add;

            exp.Execute().Invoke(accesser, new object[] { accesser});
            

            accesser.Received().Add(accesser);
        }
    }
}
