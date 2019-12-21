using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.Remote;
using Regulus.Utility;


using Console = Regulus.Utility.Console;

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
			_Controllers = new List<Controller<TUser>>();
			Factory = factory;
			_View = view;
			_Command = command;
			_Current = null;
			_Updater = new Updater();
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}

		public TUser Spawn(string name)
		{
			var user = Factory.SpawnUser();
			_Add(_Build(name, user));
			_View.WriteLine(string.Format("{0} user created.", name));

			return user;
		}

		private void _Add(Controller<TUser> controller)
		{
			_Controllers.Add(controller);
			_Updater.Add(controller.User);
		}

		private Controller<TUser> _Build(string name, TUser user)
		{
			var controller = new Controller<TUser>(name, user);
			var parser = Factory.SpawnParser(_Command, _View, controller.User);
			var builder = _CreateBuilder();
			controller.Parser = parser;
			controller.Builder = builder;
			return controller;
		}

		public void Unspawn(string name)
		{
			var controller = _Find(name);

			if(controller != null)
			{
				controller.Parser.Clear();
				if(_Current != null && _Current.User == controller.User)
				{
					_Current.Builder.Remove();
					_Current.Parser.Clear();
					_Current = null;
				}

				_Controllers.Remove(controller);
				_View.WriteLine(string.Format("{0} user removed.", name));
			}

			_View.WriteLine(string.Format("not found {0}.", name));
		}

		private Controller<TUser> _Find(string name)
		{
			var controllers = (from controller in _Controllers where controller.Name == name select controller).ToArray();
			if(controllers.Length == 1)
			{
				return controllers[0];
			}

			if(controllers.Length == 0)
			{
				return null;
			}

			throw new SystemException("controller名稱應該只有一個");
		}

		public bool Select(string name)
		{
			var controller = _Find(name);
			if(controller != null)
			{
				if(_Current != null)
				{
					_Current.Builder.Remove();
					_Current.Parser.Clear();
				}

				controller.Parser.Setup(controller.Builder);
				controller.Builder.Setup();

				_Current = controller;

				_View.WriteLine(string.Format("{0} selected.", name));
				return true;
			}

			return false;
		}

		private GPIBinderFactory _CreateBuilder()
		{
			return new GPIBinderFactory(_Command);
		}
	}
}
