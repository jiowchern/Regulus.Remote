// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoReleaseTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AutoReleaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	[TestClass]
	public class AutoReleaseTest
	{
		[TestMethod]
		public void Test()
		{
			var request = Substitute.For<IGhostRequest>();

			var ghost = new Ghost();

			var ar = new AutoRelease(request);
			ar.Register(ghost);
			ar.Update();
			ghost = null;
			GC.Collect();
			ar.Update();
			request.Received(1).Request(Arg.Any<byte>(), Arg.Any<Dictionary<byte, byte[]>>());
		}
	}
}