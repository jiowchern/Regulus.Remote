using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    partial class Levels
    {
        class ChoiceStage : Regulus.Game.IStage, IAdventureChoice
        {
            public delegate void OnToTown(string name);
            public event OnToTown ToTownEvent;
            public delegate void OnToMap(string name);
            public event OnToMap ToMapEvent;
            public delegate void OnCancel();
            public event OnCancel CancelEvent;
            ChoicePrototype _ChoicePrototype;
            Regulus.Standalong.Agent _Agent;

            ICommandable _Commandable;
            public ChoiceStage(ChoicePrototype protorype, ICommandable commandable)
            {
                _ChoicePrototype = protorype;
                _Commandable = commandable;
                _Agent = new Standalong.Agent();
            }


            void _ChoiceMap(string name)
            {
                var result = (from map in _ChoicePrototype.Maps where name == map select map).Count();
                if (result >= 1)
                {
                    ToMapEvent(name);
                }
            }
            void _ChoiceTown(string name)
            {
                var result = (from town in _ChoicePrototype.Towns where name == town select town).Count();
                if (result >= 1)
                {
                    ToTownEvent(name);
                }
            }
            void _ChoiceCancel()
            {
                if (_ChoicePrototype.Cancel)
                {
                    CancelEvent();
                }
            }
            public void Enter()
            {
                _Agent.Launch();
                _Agent.Bind<IAdventureChoice>(this);
                _Commandable.AuthorizeChoice(_Agent.QueryProvider<IAdventureChoice>().Ghosts[0]);
            }

            public void Leave()
            {
                _Commandable.InterdictChoice(_Agent.QueryProvider<IAdventureChoice>().Ghosts[0]);
                _Agent.Unbind<IAdventureChoice>(this);
                _Agent.Shutdown();
            }

            public void Update()
            {
                _Agent.Update();
            }

            string[] IAdventureChoice.Maps
            {
                get { return _ChoicePrototype.Maps; }
            }

            string[] IAdventureChoice.Town
            {
                get { return _ChoicePrototype.Towns; }
            }


            void IAdventureChoice.GoMap(string map)
            {
                _ChoiceMap(map);
            }

            void IAdventureChoice.GoTown(string tone)
            {
                _ChoiceTown(tone);
            }
        }
    }
}
