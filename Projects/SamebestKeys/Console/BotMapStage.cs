using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{


    class BotMapStage : Regulus.Utility.IStage
    {

        struct SceneBorn
        {
            public string Name;
            public Regulus.CustomType.Point[] Points;
        }

        private Regulus.Project.SamebestKeys.IUser _User;
        Regulus.Project.SamebestKeys.IOnline _Online;                
        Regulus.Project.SamebestKeys.IRealmJumper _RealmJumper;


        SceneBorn[] _SceneBorns;
        
        Regulus.Utility.StageMachine _Machine;

        public event Action<Regulus.CustomType.Point> ResultResetEvent;
        public event Action ResultConnectEvent;
        private Regulus.CustomType.Point? _BornPoint;        
        public BotMapStage(Regulus.Project.SamebestKeys.IUser _User)
        {            
            this._User = _User;
            _SceneBorns = new SceneBorn[] 
            { 
                new SceneBorn() { Name = "SC_1A" , Points = new Regulus.CustomType.Point[] 
                    {
                        new Regulus.CustomType.Point(41,420 ),
                        new Regulus.CustomType.Point(41,320 ),
                        new Regulus.CustomType.Point(37,80 ),
                        new Regulus.CustomType.Point(26,63 ),
                        new Regulus.CustomType.Point(105,290 )
                    }},
                /*new SceneBorn() { Name = "SL_1B_2" , Points = new Regulus.Types.Point[] 
                    {
                        new Regulus.Types.Point(264,438),
                        new Regulus.Types.Point(265,250),
                        new Regulus.Types.Point(82,545),
                        new Regulus.Types.Point(80,477),
                        new Regulus.Types.Point(185,247)
                    }},
                new SceneBorn() { Name = "SL_1C" , Points = new Regulus.Types.Point[] 
                    {
                        new Regulus.Types.Point(72,267),
                        new Regulus.Types.Point(128,198),
                        new Regulus.Types.Point(176,330),
                        new Regulus.Types.Point(184,115),
                        new Regulus.Types.Point(265,101)
                    }},
                new SceneBorn() { Name = "SL_1K" , Points = new Regulus.Types.Point[] 
                    {
                        new Regulus.Types.Point(99,172),
                        new Regulus.Types.Point(213,200),
                        new Regulus.Types.Point(336,182),
                        new Regulus.Types.Point(434,170),
                        new Regulus.Types.Point(534,183)
                    }},*/
            };
            _Machine = new Regulus.Utility.StageMachine();
            
        }

        public BotMapStage(Regulus.Project.SamebestKeys.IUser _User, Regulus.CustomType.Point born_point)
            : this(_User)
        {            
            this._BornPoint = born_point;            
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
        }
        

        void TraversableProvider_Supply(Regulus.Project.SamebestKeys.ITraversable obj)
        {
            obj.Ready();
            _Action();
        }        

        void ObservedAbilityProvider_Supply(Regulus.Project.SamebestKeys.IObservedAbility obj)
        {
            
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            
        }

        private void _Action()
        {
            var idx = Regulus.Utility.Random.Instance.R.NextDouble();
            if (idx > 0.1 )
            {
                _ToRun();
            }            
            else
            {
                if(_ToChangeMap() == false)
                {
                    _ToRun();
                }
            }
                

        }

        
        private bool _ToChangeMap()
        {
            if (_RealmJumper != null && _SceneBorns.Length > 0)
            {
                var idx = Regulus.Utility.Random.Next(0, _SceneBorns.Length);

                var points = _SceneBorns[idx].Points;
                var sceneName = _SceneBorns[idx].Name;
                var point = points[Regulus.Utility.Random.Next(0, points.Length)];

                _RealmJumper.Jump(sceneName);

                ResultResetEvent(point);
                return true;
            }
            return false;
        }

        private void _ToOffline()
        {
            if (_Online != null)
            {                
                _Online.Disconnect();
                ResultConnectEvent();
            }
        }

        private void _ToRun()
        {
            var stage = new BotRunStage(_User , _BornPoint );
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
            _User.PlayerProvider.Supply -= PlayerProvider_Supply;
            _User.OnlineProvider.Supply -= OnlineProvider_Supply;
            _User.TraversableProvider.Supply -= TraversableProvider_Supply;

            _Machine.Termination();
        }

        public void Update()
        {

            _Machine.Update();
            
        }

      
    }
}
