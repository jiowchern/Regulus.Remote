// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BotConnectStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the BotConnectStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

#endregion

namespace FormulaUserBot
{
	internal class BotConnectStage : IStage
	{
		public event DoneCallback DoneEvent;

		private readonly long _Id;

		private readonly string _IPAddress;

		private readonly int _Port;

		private readonly IUser _User;

		private IConnect _Con;

		public BotConnectStage(IUser user, string ip, int port, long id)
		{
			_Id = id;
			this._User = user;
			this._IPAddress = ip;
			this._Port = port;
		}

		void IStage.Leave()
		{
			_User.Remoting.OnlineProvider.Unsupply -= OnlineProvider_Unsupply;
			_User.Remoting.ConnectProvider.Supply -= _Connect;
			_User.VerifyProvider.Supply -= _Verify;
			_User.FishStageQueryerProvider.Supply -= _Query;
		}

		void IStage.Enter()
		{
			_User.FishStageQueryerProvider.Supply += _Query;
			_User.VerifyProvider.Supply += _Verify;
			_User.Remoting.ConnectProvider.Supply += _Connect;

			_User.Remoting.OnlineProvider.Unsupply += OnlineProvider_Unsupply;
		}

		void IStage.Update()
		{
		}

		public delegate void DoneCallback(IFishStage stage);

		private void OnlineProvider_Unsupply(IOnline obj)
		{
			obj_DisconnectEvent();
		}

		private void obj_DisconnectEvent()
		{
			_ConnectResult(_Con.Connect(_IPAddress, _Port));
		}

		private void _Stage(IFishStage obj)
		{
			DoneEvent(obj);
		}

		private void _Query(IFishStageQueryer obj)
		{
			_QueryResult(obj.Query(_Id, 1));
		}

		private void _QueryResult(Value<IFishStage> value)
		{
			value.OnValue += result =>
			{
				if (result == null)
				{
					throw new Exception("Stage Query null.");
				}

				this._Stage(result);
			};
		}

		private void _Verify(IVerify obj)
		{
			_VerifyResult(obj.Login("Guest", "guest"));
		}

		private void _VerifyResult(Value<bool> value)
		{
			value.OnValue += result =>
			{
				if (result == false)
				{
					throw new Exception("Verify Fail.");
				}
			};
		}

		private void _Connect(IConnect obj)
		{
			_Con = obj;
			_ConnectResult(_Con.Connect(_IPAddress, _Port));
		}

		private void _ConnectResult(Value<bool> value)
		{
			value.OnValue += result =>
			{
				if (result == false)
				{
					_ConnectResult(_Con.Connect(_IPAddress, _Port));
				}
			};
		}
	}
}