using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

namespace Regulus.Framework
{
    public class UserProvider<TUser> : Regulus.Utility.IUpdatable
        where TUser : class, Regulus.Utility.IUpdatable
    {
        List<Controller<TUser>> _Controllers;
        private IUserFactoty<TUser> Factory;
        private Utility.Console.IViewer _View;
        private Utility.Command _Command;
        Controller<TUser>? _Current;

        Regulus.Utility.Updater _Updater;
        public UserProvider(IUserFactoty<TUser> factory, Utility.Console.IViewer view, Utility.Command command)
        {
            _Controllers = new List<Controller<TUser>>();
            this.Factory = factory;
            this._View = view;
            this._Command = command;
            _Current = new Controller<TUser>?();
            _Updater = new Utility.Updater();
        }
        

        public TUser Spawn(string name)
        {
            var user = Factory.SpawnUser();
            var parser = Factory.SpawnParser(_Command, _View, user);

            _Add(_Build(name, user, parser));
            _View.WriteLine( string.Format("{0} user created." , name));
            return user;
        }

        private void _Add(Controller<TUser> controller)
        {
            _Controllers.Add(controller);
            _Updater.Add(controller.User);
        }

        private Controller<TUser> _Build(string name, TUser user , ICommandParsable<TUser> parser)
        {
            return new Controller<TUser>(name, user, parser);
        }

        public void Unspawn(string name)
        {
            var controller = _Find(name);

            if (controller.HasValue)
            {

                controller.Value.Parser.Clear();                
                if (_Current.HasValue &&  _Current.Value.User == controller.Value.User)
                    _Current = new Controller<TUser>?();

                _Controllers.Remove(controller.Value);
                _View.WriteLine(string.Format("{0} user removed.", name));
                
            }

            _View.WriteLine(string.Format("not found {0}.", name));
            
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
                    _Current.Value.Parser.Clear();       
                }
                _View.WriteLine(string.Format("{0} selected.", name));                
                _Current = controller;
                _Current.Value.Parser.Setup();                
                return true;
            }

            return false;
        }

        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void ILaunched.Launch()
        {
            
        }

        void ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }
    }
}
