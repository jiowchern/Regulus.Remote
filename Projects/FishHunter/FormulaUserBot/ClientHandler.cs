using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class ClientHandler
    {
        private string _IPAddress;
        private int _Port;        
        private int _BotAmount;
        private int _BotCount;

        public ClientHandler(string IPAddress, int Port)
        {
            // TODO: Complete member initialization
            this._IPAddress = IPAddress;
            this._Port = Port;
        }

        public ClientHandler(string IPAddress, int Port, int bot_amount)
        {
            // TODO: Complete member initialization
            this._IPAddress = IPAddress;
            this._Port = Port;
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
        }

        internal void End()
        {
            throw new NotImplementedException();
        }
    }
}
