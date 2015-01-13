using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public class Client : Regulus.Game.Framework<IUser> 
    {
        CommandBinder _CommandBinder;

        protected override Regulus.Game.Framework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new ControllerProvider[] 
            {
                new ControllerProvider(){ Command = "standalong" , Spawn = _Stand },
                new ControllerProvider(){ Command = "remoting" , Spawn = _Remoting }
            };
        }

        public Client(Regulus.Utility.Console.IViewer viewer, Regulus.Utility.Console.IInput input , Regulus.Utility.Console console)
            : base(viewer, input, console)
        {
            _CommandBinder = new CommandBinder();
        }

        private IController _Remoting()
        {
            var controller = new Regulus.Game.UserController<IUser>(new User(new Regulus.Remoting.Ghost.Native.Agent()));
            controller.LookEvent += _CommandBinder.LookUser;
            controller.UnlookEvent += _CommandBinder.UnlookUser;
            return controller;
        }

        private IController _Stand()
        {
            /*var agent = new Regulus.Standalong.Agent();
            if (_Standalong == null)
                throw new SystemException("並沒有初始化Standalong");
            _Standalong.ObtainController(agent);
            var controller = new Regulus.Game.UserController<IUser>(new User(agent));
            controller.LookEvent += _CommandBinder.LookUser;
            controller.UnlookEvent += _CommandBinder.UnlookUser;
            return controller;*/

            throw new NotImplementedException();
        }
    }
}
