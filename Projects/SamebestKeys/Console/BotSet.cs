using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Console
{
    class BotSet : Regulus.Utility.IUpdatable
    {


        Regulus.Utility.Updater _Users;
        public BotSet()
        {
            _Users = new Regulus.Utility.Updater();
        }
        bool Regulus.Utility.IUpdatable.Update()
        {
            _Users.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
        static int _Sn;
        internal void Create(int count)
        {
            if (Requester != null)
            {
                for (int i = 0; i < count; ++i)
                {
                    string account = "BotController" + (_Sn++);
                    var val = Requester.Spawn(account, false);
                    val.OnValue += (user) => { _UserCreated(user, account); };
                }
            }
        }

        private void _UserCreated(Regulus.Project.SamebestKeys.IUser user , string account)
        {
            var bot = new Bot(user, account);
            _Users.Add(bot);
        }

        public Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser>.IUserRequester Requester { private get; set; }
    }
}
