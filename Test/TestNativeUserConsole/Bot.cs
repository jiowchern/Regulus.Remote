using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestNativeUserConsole
{
    partial class Bot : Regulus.Utility.IUpdatable
    {
        private string _Name;

        public event Action ExitEvent;

        Regulus.Game.StageMachine _Machine;

        Regulus.Utility.Console.IViewer _View;
        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _ToConnect();
        }

        private void _ToConnect()
        {
            var stage = new ConnectStage(User);
            stage.ConnectResultEvent += stage_ConnectResultEvent;
            _Machine.Push(stage);
        }

        void stage_ConnectResultEvent(bool connect_result)
        {
            if (connect_result)
            {
                _ToSend();
            }
            else
            {
                ExitEvent();
            }
        }

        private void _ToSend()
        {
            var stage = new SendStage(User, _View);
            stage.ContinueEvent += (continue_send) => 
            {
                if (continue_send)
                {
                    _ToSend();
                }
                else
                {
                    ExitEvent();
                }
            };
            _Machine.Push(stage);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }

        public TestNativeUser.IUser User { get; private set; }

        

        public Bot(TestNativeUser.IUser user, string name,Regulus.Utility.Console.IViewer view)
        {
            // TODO: Complete member initialization
            this.User = user;
            this._Name = name;
            _Machine = new Regulus.Game.StageMachine();
            _View = view;
        }
    }

    partial class Bot : Regulus.Utility.IUpdatable
    {
        class ConnectStage : Regulus.Game.IStage
        {
            TestNativeUser.IUser _User;
            public event Action<bool> ConnectResultEvent;
            public ConnectStage(TestNativeUser.IUser user)
            {
                _User = user;
            }
            void Regulus.Game.IStage.Enter()
            {
                _User.ConnectProvider.Supply += ConnectProvider_Supply;
            }

            void ConnectProvider_Supply(TestNativeGameCore.IConnect obj)
            {
                var val = obj.Connect("127.0.0.1" , 12345);
                val.OnValue += val_OnValue;
            }

            void val_OnValue(bool obj)
            {
                ConnectResultEvent(obj);
            }

            void Regulus.Game.IStage.Leave()
            {
                _User.ConnectProvider.Supply -= ConnectProvider_Supply;
            }

            void Regulus.Game.IStage.Update()
            {
                
            }
        }
    }

    partial class Bot : Regulus.Utility.IUpdatable
    {
        class SendStage : Regulus.Game.IStage
        {
            TestNativeUser.IUser _User;
            Regulus.Utility.Console.IViewer _View;
            public event Action<bool> ContinueEvent;
            public SendStage(TestNativeUser.IUser user, Regulus.Utility.Console.IViewer view)
            {
                _User = user;
                _View = view;
            }
            void Regulus.Game.IStage.Enter()
            {
                _User.MessagerProvider.Supply += MessagerProvider_Supply;
            }

            void MessagerProvider_Supply(TestNativeGameCore.IMessager obj)
            {
                var val = obj.Send(_BuildString());
                val.OnValue += val_OnValue;
            }

            void val_OnValue(string respunse)
            {
                _View.WriteLine(respunse);
                ContinueEvent( true );
            }

            private string _BuildString()
            {
                //return "1";
                return System.DateTime.Now.ToString();
            }

            void Regulus.Game.IStage.Leave()
            {
                
            }

            void Regulus.Game.IStage.Update()
            {
                
            }
        }
    }
}
