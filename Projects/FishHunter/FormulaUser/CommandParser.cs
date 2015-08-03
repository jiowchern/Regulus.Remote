// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandParser.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CommandParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Extension;
using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public class CommandParser : ICommandParsable<IUser>
	{
		private readonly Command _Command;

		private readonly IUser _User;

		private readonly Console.IViewer _View;

		public CommandParser(Command command, Console.IViewer view, IUser user)
		{
			this._Command = command;
			this._View = view;
			this._User = user;
		}

		void ICommandParsable<IUser>.Clear()
		{
			_Command.Unregister("Package");
		}

		void ICommandParsable<IUser>.Setup(IGPIBinderFactory factory)
		{
			var connect = factory.Create(_User.Remoting.ConnectProvider);
			connect.Bind("Connect[result , ipaddr ,port]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, int, bool>(gpi.Connect, _ConnectResult); });

			var online = factory.Create(_User.Remoting.OnlineProvider);
			online.Bind("Ping", 
				gpi => { return new CommandParamBuilder().Build(() => { _View.WriteLine("Ping : " + gpi.Ping); }); });

			var verify = factory.Create(_User.VerifyProvider);
			verify.Bind("Login[result,id,password]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, string, bool>(gpi.Login, _VerifyResult); });

			var fishStageQueryer = factory.Create(_User.FishStageQueryerProvider);
			fishStageQueryer.Bind("Query[result,player_id,fish_stage]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<long, byte, IFishStage>(gpi.Query, _QueryResult); });

			_Command.Register("Package", _ShowPackageState);
		}

		private void _ConnectResult(bool result)
		{
			_View.WriteLine(string.Format("Connect result {0}", result));
		}

		private void _ShowPackageState()
		{
			_View.WriteLine(string.Format("Request Queue:{0} \tResponse Queue:{1}", Agent.RequestPackages, Agent.ResponsePackages));
		}

		private void fishStage_UnsupplyEvent(IFishStage source)
		{
			source.HitResponseEvent -= source_HitResponseEvent;
		}

		private void fishStage_SupplyEvent(IFishStage source)
		{
			source.HitResponseEvent += source_HitResponseEvent;
		}

		private void source_HitResponseEvent(HitResponse response)
		{
			_View.WriteLine("Hit response : " + response.ShowMembers());
		}

		private void _QueryResult(IFishStage result)
		{
			_View.WriteLine(string.Format("Query fish stage result {0}", result.ShowMembers()));
		}

		private void _VerifyResult(bool result)
		{
			_View.WriteLine(string.Format("Verify result {0}", result));
		}
	}
}