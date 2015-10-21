using System;
using System.Linq;
using System.Net.Mime;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using ProtoBuf;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace GameTest
{
	[TestClass]
	public class DataTest
	{
		[TestMethod]
		public void TestAccountValid()
		{
			var account = new Account();

			Assert.AreNotEqual(null, account.Competnces);
			Assert.AreNotEqual(null, account.Name);
			Assert.AreNotEqual(null, account.Id);
			Assert.AreNotEqual(null, account.Password);
		}

		[TestMethod]
		public void TestValidRecord()
		{
			var record = new GamePlayerRecord();

			Assert.AreNotEqual(null, record.Id);
			Assert.AreEqual(0, record.Money);
			Assert.AreEqual(Guid.Empty, record.Owner);
		}

		[TestMethod]
		public void TestStageLock()
		{
			var stageLock = new StageLock();

			Assert.AreEqual(0, stageLock.KillCount);
			Assert.AreNotEqual(null, stageLock.Requires);
			Assert.AreEqual(0, stageLock.Stage);
		}


		[TestMethod]
		public void Serializer()
		{
			RequsetFishData data1 = new RequsetFishData();
			data1.GraveGoods = new RequsetFishData[0];

			var serData = Regulus.TypeHelper.Serializer(data1);

			var data2 = Regulus.TypeHelper.Deserialize<RequsetFishData>(serData);
			

			Assert.AreEqual(data2.GraveGoods, null);
		}

		[ProtoContract]
		public class Data
		{
			[ProtoContract]
			public class T
			{
				[ProtoMember(1)]
				public int t1;
				[ProtoMember(2)]
				public int t2;

				public T()
				{
					t1 = 123;
					t2 = 456;
				}
			}

			[ProtoMember(1)]
			public int[] ary;

			[ProtoMember(2)]
			public T[] Tary;

			public Data()
			{
				//ary = new int [0];
				//Tary = new T[] {new T() };
			}
		}
		[TestMethod]
		public void DeSerializer2()
		{
			Data data1 = new Data();

			data1.ary = new int[2];

			var serData = Regulus.TypeHelper.Serializer(data1);

			var data2 = Regulus.TypeHelper.Deserialize<Data>(serData);


			Assert.AreEqual(data2.ary.Length, 1);
		}

		[TestMethod]
		public void DeSerializer()
		{
			FishFarmData data1 = new FishFarmData();

			var serData = Regulus.TypeHelper.Serializer(data1);

			var data2 = Regulus.TypeHelper.Deserialize<FishFarmData>(serData);


			Assert.AreEqual(data2.DataRootRoots.Length, 0);
		}
	}
}
