﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verify.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UnitTest1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.Remoting;

using VGame.Project.FishHunter;
using VGame.Project.FishHunter.Common;

#endregion

namespace UnitTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestVerifySuccess()
		{
			// data 
			var id = "12345678";
			var pw = "0000";
			var stroage = Substitute.For<IAccountFinder>();
			stroage.FindAccountByName(id).Returns(new Value<Account>(new Account
			{
				Name = id, 
				Password = pw
			}));

			var returnValue = false;
			var eventValue = false;
			var stage = new Verify(stroage);
			stage.OnDoneEvent += account => { eventValue = true; };

			IVerify verify = stage;

			// test
			var val = verify.Login("12345678", "0000");

			// data
			val.OnValue += result => { returnValue = result; };

			// verify
			Assert.AreEqual(true, returnValue);
			Assert.AreEqual(true, eventValue);
		}

		[TestMethod]
		public void TestVerifyFail()
		{
			var id = "12345678";
			var pw = "0000";
			var stroage = Substitute.For<IAccountFinder>();
			stroage.FindAccountByName(id).Returns(new Value<Account>(new Account
			{
				Name = id, 
				Password = pw
			}));

			var returnValue = false;
			var eventValue = false;
			var stage = new Verify(stroage);
			stage.OnDoneEvent += account => { eventValue = true; };

			IVerify verify = stage;
			var val = verify.Login("12345678", "011111001");
			val.OnValue += result => { returnValue = result; };

			Assert.AreEqual(false, returnValue);
			Assert.AreEqual(false, eventValue);
		}
	}
}