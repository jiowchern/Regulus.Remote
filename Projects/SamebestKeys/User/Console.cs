using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public class Console : Regulus.Game.Framework<Regulus.Project.SamebestKeys.IUser>
    {
        Regulus.Projects.SamebestKeys.Standalong.Game _Game;
        private IController _BuildRemoting()
        {            
            return new UserController(_Viewer, Command , new Regulus.Projects.SamebestKeys.Remoting.RemotingUser());
        }

        private IController _Standalong()
        {
            return new UserController(_Viewer, Command, new Regulus.Projects.SamebestKeys.Standalong.StandalongUser(_Game));
        }
        

        public Console(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view, input)
        {
            _Game = new Projects.SamebestKeys.Standalong.Game();
        }

        protected override Regulus.Game.Framework<Regulus.Project.SamebestKeys.IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new Console.ControllerProvider[] 
            {                
                new Console.ControllerProvider { Command = "remoting" , Spawn = _BuildRemoting},
                new Console.ControllerProvider { Command = "standalong" , Spawn = _Standalong}
            };
        }

        protected override void _Launch(Regulus.Utility.Updater updater)
        {
            updater.Add(_Game);
        }
        protected override void _Shutdown(Regulus.Utility.Updater updater) 
        {
            updater.Remove(_Game);
        }

        public GameDataBuilder DataBuilder
        {
            get { return new GameDataBuilder(GameData.Instance); }
        }
        
    }
}
