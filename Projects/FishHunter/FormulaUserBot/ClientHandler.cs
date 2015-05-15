using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class ClientHandler : Regulus.Utility.IUpdatable
    {
        private string _IPAddress;
        private int _Port;        
        private int _BotAmount;
        private int _BotCount;

        Regulus.Utility.CenterOfUpdateable _Bots;

        public ClientHandler(string IPAddress, int Port)
        {
            // TODO: Complete member initialization
            this._IPAddress = IPAddress;
            this._Port = Port;
            _Bots = new Regulus.Utility.CenterOfUpdateable();
        }

        public ClientHandler(string IPAddress, int Port, int bot_amount)
            : this(IPAddress, Port)
        {
            
            this._BotAmount = bot_amount;
        }

        internal void Begin(Regulus.Framework.GameModeSelector<VGame.Project.FishHunter.Formula.IUser> selector)
        {
            selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.RemotingUserFactory());
            _OnProvider(selector.CreateUserProvider("remoting"));
        }

        private void _OnProvider(Regulus.Framework.UserProvider<VGame.Project.FishHunter.Formula.IUser> userProvider)
        {
            while (_BotCount < _BotAmount)
            {
                _OnUser(userProvider.Spawn("bot" + _BotCount));
                _BotCount++;
            }

        }

        private void _OnUser(VGame.Project.FishHunter.Formula.IUser user)
        {
            var bot = new Bot(_IPAddress, _Port , user);
            _Bots.Add(bot);
        }

        internal void End()
        {
            _Bots.Shutdown();
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Bots.Working();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
