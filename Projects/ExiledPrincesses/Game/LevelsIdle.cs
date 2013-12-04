using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    partial class Levels
    {

        public class IdleStage : Regulus.Game.IStage, IAdventureIdle
        {
            public delegate void OnGoForward();
            public event OnGoForward GoForwardEvent;

            ICommandable _Commandable;
            Regulus.Standalong.Agent _Agent;
            IdleStage()
            {
                _Agent = new Standalong.Agent();
            }
            public IdleStage(ICommandable commandable)
                : this()
            {
                _Commandable = commandable;
            }
            void Regulus.Game.IStage.Enter()
            {
                _Agent.Launch();
                _Agent.Bind<IAdventureIdle>(this);
                _Commandable.AuthorizeIdle(Get());

            }
            public IAdventureIdle Get()
            {
                var provider = _Agent.QueryProvider<IAdventureIdle>();
                return provider.Ghosts[0];
            }

            void _GoForwar()
            {
                if (GoForwardEvent != null)
                    GoForwardEvent();
                GoForwardEvent = null;
            }

            void Regulus.Game.IStage.Leave()
            {
                _Commandable.InterdictIdle(null);
                _Agent.Unbind<IAdventureIdle>(this);
                _Agent.Shutdown();
            }

            void Regulus.Game.IStage.Update()
            {
                _Agent.Update();
            }

            void IAdventureIdle.GoForwar()
            {
                _GoForwar();
            }
        }
    }
}
