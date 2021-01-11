
using System;

namespace Regulus.Tool.GPI
{
    public interface GPIA
    {
        void Method();
        Guid Id { get; }
        Regulus.Remote.Value<bool> MethodReturn();

        event Action<float, string> OnCallEvent;
    }

}
namespace Regulus.Tool.Tests
{
    
    public class GhostProviderGeneratorTests
    {





        [Xunit.Fact]
        public void BuildGetEventHandler()
        {
            bool onevent = false;

            Delegate function = new Action<int, float, string>((i, f, s) => { onevent = true; });
            function.Method.Invoke(function.Target, new object[] { 10, 100f, "1000" });

            Xunit.Assert.True(onevent);            
        }


    }
}