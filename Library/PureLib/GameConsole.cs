using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

namespace Regulus.Framework
{
    public class GameConsole<TUser>
        where TUser : class
    {
        List<Controller<TUser>> _Controllers;
        private IUserFactoty<TUser> Factory;
        private Utility.Console.IViewer _View;
        private Utility.Command _Command;
        Controller<TUser>? _Current;        

        ICommandParsable<TUser> _Parser;

        public GameConsole(ICommandParsable<TUser> parser , IUserFactoty<TUser> factory, Utility.Console.IViewer view, Utility.Command command)
            : this(factory, view, command)
        {
            _Parser = parser;
        }
        GameConsole(IUserFactoty<TUser> factory, Utility.Console.IViewer view, Utility.Command command)
        {
            _Controllers = new List<Controller<TUser>>();
            this.Factory = factory;
            this._View = view;
            this._Command = command;
            _Current = new Controller<TUser>?();
        }
        

        public TUser Spawn(string name)
        {
            var user = Factory.Spawn();            

            _Add(_Build(name, user));

            return user;
        }

        private void _Add(Controller<TUser> controller)
        {
            _Controllers.Add(controller);
        }

        private Controller<TUser> _Build(string name, TUser user)
        {
            return new Controller<TUser>(name, user);
        }

        public void Unspawn(string name)
        {
            var controller = _Find(name);

            if (controller.HasValue)
            {
                _Parser.Unregister(controller.Value.User, _Command);
                if (_Current.HasValue &&  _Current.Value.User == controller.Value.User)
                    _Current = new Controller<TUser>?();

                _Controllers.Remove(controller.Value);
            }
        }

        private Controller<TUser>? _Find(string name)
        {
            var controllers =  (from controller in _Controllers where controller.Name == name select controller).ToArray();
            if (controllers.Length == 1)
                return controllers[0];
            else if (controllers.Length == 0)
            {
                return new Controller<TUser>?();
            }

            throw new SystemException("controller名稱應該只有一個");
        }

        public bool Select(string name)
        {
            var controller = _Find(name);
            if (controller.HasValue)
            {
                if (_Current.HasValue)
                {
                    _Parser.Unregister(_Current.Value.User, _Command);
                }                    
                _Parser.Register(controller.Value.User , _Command );

                _Current = controller;
                return true;
            }

            return false;
        }
    }
}
