using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestNativeUserConsole
{
    using Regulus.Extension;


    
    public class Appliaction : Regulus.Utility.IUpdatable
    {
        private TestNativeUser.Application _Appliaction;
        Regulus.Game.ConsoleFramework<TestNativeUser.IUser>.IUserRequester _UserRequester;
        Regulus.Game.ConsoleFramework<TestNativeUser.IUser>.ISystemSelector _SystemSelector;

        Regulus.Utility.Updater _Bots;
        int _BotAmount;
        long _BotSn;
        Regulus.Utility.Console.IViewer _View;
        public Appliaction(TestNativeUser.Application appliaction , Regulus.Utility.Console.IViewer view)
        {
            _Bots = new Regulus.Utility.Updater();
            this._Appliaction = appliaction;
            _View = view;
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Bots.Update();

            if(_BotAmount >0 )
            {
                _BotAmount--;
                var name = "jc" + _BotSn.ToString();
                var userValue =  _UserRequester.Spawn( name, false);
                userValue.OnValue += (user) =>
                {
                    var bot = _CreateBot(user, name);
                    bot.ExitEvent += () => 
                    {
                        _UserRequester.Unspawn(name);
                        _RestoryBot(bot.User); 
                    };
                };
                _BotSn++;
            }
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
            _Appliaction.SelectSystemEvent += _Appliaction_SelectSystemEvent;            

            _Appliaction.Command.Register("one" , _BuildOne );
            _Appliaction.Command.Register<int,string,int>("bot", _BuildBot);
        }
        string _Ip;
        int _Port;
        private void _BuildBot(int count,string ip,int port)
        {
            _Ip = ip;
            _Port = port;
            _BotAmount = count;
            
            var val = _SystemSelector.Use("remoting");
            val.OnValue += (requester) => 
            {
                _UserRequester = requester;                
            };
        }

        void _RestoryBot(TestNativeUser.IUser user)
        {
            foreach(var bot in _Bots.Objects)
            {
                if ((bot as Bot).User == user)
                {
                    _Bots.Remove(bot);
                    _BotAmount++;
                }                
            }
            
        }

        Bot _CreateBot(TestNativeUser.IUser user,string name)
        {
            var bot = new Bot(user , name , _View,_Ip , _Port);            
            _Bots.Add(bot);
            
            return bot;
        }

        private void _BuildOne()
        {
            var val = _SystemSelector.Use("remoting");
            val.OnValue += _OnUserRequester;

        }

        void _OnUserRequester(Regulus.Game.ConsoleFramework<TestNativeUser.IUser>.IUserRequester obj)
        {
            if (obj != null)
            {
                obj.Spawn("jc", true);
            }
        }

        

        void _Appliaction_SelectSystemEvent(Regulus.Game.ConsoleFramework<TestNativeUser.IUser>.ISystemSelector system_selector)
        {
            _SystemSelector = system_selector;
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Regulus.Utility.Console.IViewer viwer = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(viwer);
            TestNativeUser.Application appliaction = new TestNativeUser.Application(viwer, input);

            var app = new Appliaction(appliaction, viwer);

            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            appliaction.SetLogMessage(Regulus.Utility.Console.LogFilter.All);
            updater.Add(app);
            updater.Add(appliaction);
            
            bool exit = false;
            appliaction.Command.Register("quit", () => { exit = true; });
            
            while (exit == false)
            {
                input.Update();
                updater.Update();
                
            }
            appliaction.Command.Unregister("quit");
        }
        
    }
   
}
