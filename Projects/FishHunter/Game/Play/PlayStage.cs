// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayStage.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the PlayStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Play
{
	internal class PlayStage : IPlayer, IStage
	{
		private event Action<int> _DeathFishEvent;

		private event Action<int> _MoneyEvent;

		public event KillCallback KillEvent;

		public event PassCallback PassEvent;

		private readonly ISoulBinder _Binder;

		private readonly List<Bullet> _Bullets;

		private readonly List<Fish> _Fishs;

		private readonly IFishStage _FishStage;

		private readonly Record _Money;

		private readonly Dictionary<int, HitRequest> _Requests;

		private BULLET _Bullet;

		private int _DeadFishCount;

		private short _FishIdSn;

		private int _WeaponOdds;

		public PlayStage(ISoulBinder binder, IFishStage fish_stage, Record money)
		{
			this._Fishs = new List<Fish>();
			this._Bullets = new List<Bullet>();
			this._Requests = new Dictionary<int, HitRequest>();
			this._Binder = binder;
			this._FishStage = fish_stage;
			this._DeadFishCount = 0;
			this._Money = money;
			this._Bullet = BULLET.WEAPON1;
			_WeaponOdds = 10;
		}

		Value<int> IPlayer.Hit(int bulletid, int[] fishids)
		{
			var hasBullet = this._PopBullet(bulletid);
			if (hasBullet == false)
			{
				return 0;
			}

			var logFishs = string.Empty;
			var count = 0;
			foreach (var fishid in fishids)
			{
				if (this._PopFish(fishid) == false)
				{
					continue;
				}

				if (this._Requests.ContainsKey(fishid))
				{
					continue;
				}

				count++;
				var request = new HitRequest
				{
					FishID = (short)fishid, 
					FishOdds = 1, 
					FishStatus = FISH_STATUS.NORMAL, 
					FishType = 1, 
					TotalHits = (short)fishids.Length, 
					HitCnt = (short)count, 
					TotalHitOdds = 1, 
					WepBet = 1, 
					WepID = (short)bulletid, 
					WepOdds = (short)_WeaponOdds, 
					WepType = (byte)this._Bullet
				};

				_Requests.Add(fishid, request);
				_FishStage.Hit(request);

				logFishs += fishid + ",";
			}

			if (count == 0)
			{
				this._PushBullet(bulletid);
			}

			Singleton<Log>.Instance.WriteInfo(
				string.Format(
					"all bullet:{0} , targets:{1} , count:{2}", 
					bulletid, 
					string.Join(",", (from id in fishids select id.ToString()).ToArray()), 
					fishids.Length));
			Singleton<Log>.Instance.WriteInfo(
				string.Format("requested bullet:{0} , targets:{1} , count:{2}", bulletid, logFishs, count));
			Singleton<Log>.Instance.WriteInfo(
				string.Format(
					"request fishs:{0} count:{1} ", 
					string.Join(",", (from id in this._Requests.Keys select id.ToString()).ToArray()), 
					this._Requests.Count));
			return count;
		}

		void IPlayer.Quit()
		{
			this.KillEvent(this._DeadFishCount);
		}

		event Action<int> IPlayer.DeathFishEvent
		{
			add { this._DeathFishEvent += value; }

			remove { this._DeathFishEvent -= value; }
		}

		event Action<int> IPlayer.MoneyEvent
		{
			add { this._MoneyEvent += value; }

			remove { this._MoneyEvent -= value; }
		}

		int IPlayer.WeaponOdds
		{
			get { return _WeaponOdds; }
		}

		BULLET IPlayer.Bullet
		{
			get { return this._Bullet; }
		}

		Value<int> IPlayer.RequestBullet()
		{
			if (this.HasMoney(1) == false)
			{
				return 0;
			}

			this.AddMoney(-1);

			return this._CreateBullet();
		}

		Value<short> IPlayer.RequestFish()
		{
			checked
			{
				var fishid = ++this._FishIdSn;
				this._Fishs.Add(new Fish(fishid));
				return fishid;
			}
		}

		void IPlayer.EquipWeapon(BULLET bullet, int odds)
		{
			this._Bullet = bullet;
			_WeaponOdds = odds;
		}

		void IStage.Enter()
		{
			this._Binder.Bind<IPlayer>(this);
			this._FishStage.OnHitResponseEvent += this._Response;
		}

		void IStage.Leave()
		{
			this._FishStage.OnHitResponseEvent -= this._Response;
			this._Binder.Unbind<IPlayer>(this);
		}

		void IStage.Update()
		{
		}

		public delegate void PassCallback(int pass_stage);

		public delegate void KillCallback(int kill_count);

		private void _Response(HitResponse obj)
		{
			HitRequest request;
			if (this._Requests.TryGetValue(obj.FishID, out request))
			{
				if (obj.DieResult == FISH_DETERMINATION.DEATH)
				{
					var onDeathFish = this._DeathFishEvent;
					if (onDeathFish != null)
					{
						onDeathFish(obj.FishID);
					}

					this.AddMoney(request.WepBet * request.WepOdds);
					this._DeadFishCount++;
				}
				else if (obj.DieResult == FISH_DETERMINATION.SURVIVAL)
				{
					this._PushFish(obj.FishID);
				}

				this._Requests.Remove(obj.FishID);
			}
		}

		private void _PushFish(short id)
		{
			this._Fishs.Add(new Fish(id));
		}

		private bool _PopFish(int fishid)
		{
			return _Fishs.RemoveAll(fish => fish.Id == fishid) > 0;
		}

		private void _PushBullet(int bulletid)
		{
			this._Bullets.Add(new Bullet(bulletid));
		}

		private bool _PopBullet(int bulletid)
		{
			return this._Bullets.RemoveAll(b => b.Id == bulletid) > 0;
		}

		private void AddMoney(int p)
		{
			this._Money.Money += p;
			this._MoneyEvent(this._Money.Money);
		}

		private bool HasMoney(short bullet_score)
		{
			return this._Money.Money >= bullet_score;
		}

		private Value<int> _CreateBullet()
		{
			var bullet = new Bullet();
			this._Bullets.Add(bullet);
			return bullet.Id;
		}
	}
}