using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    

    public class Client : Regulus.Game.Framework<IUser> 
    {
        CommandBinder _CommandBinder;
        Regulus.Game.ICore _Standalong;
        protected override Regulus.Game.Framework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new ControllerProvider[] 
            {
                new ControllerProvider(){ Command = "standalong" , Spawn = _Stand },
                new ControllerProvider(){ Command = "remoting" , Spawn = _Remoting }
            };
        }
        public Client(Regulus.Utility.Console.IViewer viewer, Regulus.Utility.Console.IInput input)
            : base(viewer, input)
        {
            _CommandBinder = new CommandBinder(viewer, Command);
        }
        public Client(Regulus.Game.ICore standalong_core,Regulus.Utility.Console.IViewer viewer, Regulus.Utility.Console.IInput input)
            : this(viewer, input)
        {
            _Standalong = standalong_core;
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
            var agent = new Regulus.Standalong.Agent();
            if (_Standalong == null)
                throw new SystemException("並沒有初始化Standalong");
            _Standalong.ObtainController(agent);
            var controller = new Regulus.Game.UserController<IUser>(new User(agent));
            controller.LookEvent += _CommandBinder.LookUser;
            controller.UnlookEvent += _CommandBinder.UnlookUser;
            return controller;
        }

        
    }
}
