// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandCall.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CommandCall type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.Remoting;
using Regulus.Remoting.Extension;
using Regulus.Utility;

#endregion

namespace RemotingTest
{
	[TestClass]
	public class CommandCall
	{
		[TestMethod]
		public void TestCommandCall()
		{
			var param = Substitute.For<CommandParam>();
			var called = false;
			param.Types = new[]
			{
				typeof (string)
			};
			param.Callback = new Action<string>(msg => { called = true; });

			var command = new Command();
			command.Register("123", param);
			command.Run("123", new[]
			{
				" Hello World."
			});

			Assert.AreEqual(true, called);
		}

		[TestMethod]
		public void TestCommandAdd()
		{
			var param = Substitute.For<CommandParam>();
			float value = 0;
			param.Types = new[]
			{
				typeof (int), 
				typeof (int)
			};
			param.ReturnType = typeof (float);

			param.Callback = new Func<int, int, float>((a, b) => { return a + b; });
			param.Return = new Action<float>(val => { value = val; });

			var command = new Command();
			command.Register("123", param);
			command.Run("123", new[]
			{
				"1", 
				"2"
			});

			Assert.AreEqual(3, value);
		}

		[TestMethod]
		public void TestGPIBinder()
		{
			var command = new Command();
			var notifier = Substitute.For<INotifier<IBinderTest>>();

			var binder = new GPIBinder<IBinderTest>(notifier, command);

			binder.Bind(tester => tester.Function1());

			binder.Bind<int>((tester, arg) => tester.Function2(arg));

			binder.Bind(tester => tester.Function3(), ret => { });
		}
	}
}