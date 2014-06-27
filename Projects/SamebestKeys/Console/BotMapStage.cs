using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{


    class BotMapStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        Regulus.Project.SamebestKeys.IOnline _Online;        
        Regulus.Project.SamebestKeys.IPlayer _Player;
        Regulus.Project.SamebestKeys.IObservedAbility _Observed;
        Regulus.Project.SamebestKeys.IRealmJumper _RealmJumper;
        
        string[] _Scenes;
        Regulus.Game.StageMachine _Machine;
        public enum Result
        {
            Connect,Reset
        };
        public event Action<Result> ResultEvent;
        public BotMapStage(Regulus.Project.SamebestKeys.IUser _User)
        {            
            this._User = _User;
            _Scenes = new string[0];
            _Machine = new Regulus.Game.StageMachine();
        }
        public void Enter()
        {
            _User.TraversableProvider.Supply += TraversableProvider_Supply;
            _User.OnlineProvider.Supply += OnlineProvider_Supply;
            _User.PlayerProvider.Supply += PlayerProvider_Supply;
            _User.RealmJumperProvider.Supply += RealmJumperProvider_Supply;
            


            
        }

        void RealmJumperProvider_Supply(Regulus.Project.SamebestKeys.IRealmJumper obj)
        {
            _RealmJumper = obj;

            _RealmJumper.Query().OnValue += (scenes) => { _Scenes = scenes; };
        }

        void LevelSelectorProvider_Supply(Regulus.Project.SamebestKeys.ILevelSelector obj)
        {
            
        }

        void TraversableProvider_Supply(Regulus.Project.SamebestKeys.ITraversable obj)
        {
            obj.Ready();            
        }        

        void ObservedAbilityProvider_Supply(Regulus.Project.SamebestKeys.IObservedAbility obj)
        {
            if (obj.Id == _Player.Id)
            {
                _Observed = obj;
                _Action();
            }
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            _Player = obj;
            _User.ObservedAbilityProvider.Supply += ObservedAbilityProvider_Supply;
        }

        private void _Action()
        {
            var idx = Regulus.Utility.Random.Instance.R.NextDouble();
            if (idx > 0.5 )
            {
                _ToRun();
            }
            else if (idx > 0.4 && idx < 0.5)
            {
                _ToTalk();
            }
            else if (idx > 0.2 && idx < 0.4 )
            {
                _ToEmo();
            }
            else if (idx > 0.01 && idx < 0.02)
            {
                //_ToRun();
                _ToChangeMap();
            }
            else
                _Action();

        }

        private void _ToTalk()
        {
            var stage = new BotTalkStage(_Player);
            stage.DoneEvent += _Action;
            _Machine.Push(stage);
        }

        private void _ToEmo()
        {
            var stage = new BotEmoStage(_Player);
            stage.DoneEvent += _Action;
            _Machine.Push(stage);
        }

        private void _ToChangeMap()
        {
            if (_Player != null && _Scenes.Length > 0)
            {
                _Player.Stop(0);

                var idx = Regulus.Utility.Random.Next(0, _Scenes.Length);
                _RealmJumper.Jump(_Scenes[idx]);

                ResultEvent(Result.Reset);
            }
        }

        private void _ToOffline()
        {
            if (_Online != null)
            {                
                _Online.Disconnect();
                ResultEvent(Result.Connect);
            }
        }

        private void _ToRun()
        {
            var stage = new BotRunStage(_Player , _Observed);
            stage.DoneEvent += _Action;
            _Machine.Push(stage);
        }

        void OnlineProvider_Supply(Regulus.Project.SamebestKeys.IOnline obj)
        {
            _Online = obj;
        }

        public void Leave()
        {
            _User.RealmJumperProvider.Supply += RealmJumperProvider_Supply;
            _User.ObservedAbilityProvider.Supply -= ObservedAbilityProvider_Supply;
            _User.PlayerProvider.Supply -= PlayerProvider_Supply;
            _User.OnlineProvider.Supply -= OnlineProvider_Supply;
            _User.TraversableProvider.Supply -= TraversableProvider_Supply;

            _Machine.Termination();
        }

        public void Update()
        {

            /*if (_TimeCounter.Second > _Timeup)
            {
                if (_Player != null)                
                {
                    _Player.Stop(0);
                    var map = Regulus.Utility.Random.Next(0, 3);
                    if (map == 0)
                    {
                        _Player.Goto("Ark", Regulus.Utility.Random.Next(47, 47 + 10), Regulus.Utility.Random.Next(167, 167+10));
                    }
                    if (map == 1)
                        _Player.Goto("Test", Regulus.Utility.Random.Next(50, 50 + 10), Regulus.Utility.Random.Next(50, 50 + 10));
                    if (map == 2)
                        _Player.Goto("SL_1C", Regulus.Utility.Random.Next(169, 169 + 10), Regulus.Utility.Random.Next(148, 148 + 10));

                    
                }
                    
                if (_Online != null)
                {
                    
                    var result = Regulus.Utility.Random.Next(0, 10) >= 9 ? Result.Connect : Result.Reset;
                    if (result == Result.Connect)
                    {
                        _Online.Disconnect();
                    }
                    ResultEvent(result);
                }
            }*/

            _Machine.Update();
            
        }

      
    }
}
