// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UserProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Remoting;
using Regulus.Utility;

using Console = Regulus.Utility.Console;

#endregion

namespace Regulus.Framework
{
	public class UserProvider<TUser> : IUpdatable
		where TUser : class, IUpdatable
	{
		private readonly Command _Command;

		private readonly List<Controller<TUser>> _Controllers;

		private readonly Updater _Updater;

		private readonly Console.IViewer _View;

		private readonly IUserFactoty<TUser> Factory;

		private Controller<TUser> _Current;

		public UserProvider(IUserFactoty<TUser> factory, Console.IViewer view, Command command)
		{
			this._Controllers = new List<Controller<TUser>>();
			this.Factory = factory;
			this._View = view;
			this._Command = command;
			this._Current = null;
			this._Updater = new Updater();
		}

		bool IUpdatable.Update()
		{
			this._Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
		}

		void IBootable.Shutdown()
		{
			this._Updater.Shutdown();
		}

		public TUser Spawn(string name)
		{
			var user = this.Factory.SpawnUser();
			this._Add(this._Build(name, user));
			this._View.WriteLine(string.Format("{0} user created.", name));

			return user;
		}

		private void _Add(Controller<TUser> controller)
		{
			this._Controllers.Add(controller);
			this._Updater.Add(controller.User);
		}

		private Controller<TUser> _Build(string name, TUser user)
		{
			var controller = new Controller<TUser>(name, user);
			var parser = this.Factory.SpawnParser(this._Command, this._View, controller.User);
			var builder = this._CreateBuilder();
			controller.Parser = parser;
			controller.Builder = builder;
			return controller;
		}

		public void Unspawn(string name)
		{
			var controller = this._Find(name);

			if (controller != null)
			{
				controller.Parser.Clear();
				if (this._Current != null && this._Current.User == controller.User)
				{
					this._Current.Builder.Remove();
					this._Current.Parser.Clear();
					this._Current = null;
				}

				this._Controllers.Remove(controller);
				this._View.WriteLine(string.Format("{0} user removed.", name));
			}

			this._View.WriteLine(string.Format("not found {0}.", name));
		}

		private Controller<TUser> _Find(string name)
		{
			var controllers = (from controller in this._Controllers where controller.Name == name select controller).ToArray();
			if (controllers.Length == 1)
			{
				return controllers[0];
			}

			if (controllers.Length == 0)
			{
				return null;
			}

			throw new SystemException("controller名稱應該只有一個");
		}

		public bool Select(string name)
		{
			var controller = this._Find(name);
			if (controller != null)
			{
				if (this._Current != null)
				{
					this._Current.Builder.Remove();
					this._Current.Parser.Clear();
				}

				controller.Parser.Setup(controller.Builder);
				controller.Builder.Setup();

				this._Current = controller;

				this._View.WriteLine(string.Format("{0} selected.", name));
				return true;
			}

			return false;
		}

		private GPIBinderFactory _CreateBuilder()
		{
			return new GPIBinderFactory(this._Command);
		}
	}
}