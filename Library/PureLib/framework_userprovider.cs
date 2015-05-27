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
        Controller<TUser> _Current;

        Regulus.Utility.CenterOfUpdateable _Updater;
        public UserProvider(IUserFactoty<TUser> factory, Utility.Console.IViewer view, Utility.Command command)
        {
            _Controllers = new List<Controller<TUser>>();
            this.Factory = factory;
            this._View = view;
            this._Command = command;
            _Current = null;
            _Updater = new Utility.CenterOfUpdateable();
        }
        

        public TUser Spawn(string name)
        {
            var user = Factory.SpawnUser();
            _Add(_Build(name, user));
            _View.WriteLine( string.Format("{0} user created." , name));

            


            return user;
        }

        private void _Add(Controller<TUser> controller)
        {
            _Controllers.Add(controller);
            _Updater.Add(controller.User);
        }

        private Controller<TUser> _Build(string name, TUser user )
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

            if (controller != null)
            {
                controller.Parser.Clear();                
                if (_Current != null &&  _Current.User == controller.User)
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
            var controllers =  (from controller in _Controllers where controller.Name == name select controller).ToArray();
            if (controllers.Length == 1)
                return controllers[0];
            else if (controllers.Length == 0)
            {
                return null;
            }

            throw new SystemException("controller名稱應該只有一個");
        }

        public bool Select(string name)
        {
            var controller = _Find(name);
            if (controller != null)
            {
                if (_Current != null)
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

        private Regulus.Remoting.GPIBinderFactory _CreateBuilder()
        {
            return new Remoting.GPIBinderFactory(_Command);
        }

        bool Utility.IUpdatable.Update()
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
    }
}
