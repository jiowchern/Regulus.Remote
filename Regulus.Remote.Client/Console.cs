using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Client
{
    public class Console : Regulus.Utility.WindowConsole
    {
        
        private readonly Type[] _WatchTypes;
        private readonly IAgentProvider _AgentProvider;
        readonly Regulus.Utility.Updater _Updater;
        readonly List<User> _Users;

        readonly Regulus.Remote.Client.AgentCommandRegister _Register;

        public Console(IEnumerable<Type> watch_types , IAgentProvider agent_provider, Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input) : base(view , input)
        {
        
            _WatchTypes = watch_types.Union( new Type[] { typeof(IConnect) , typeof(IOnline)} ).ToArray();
            _AgentProvider = agent_provider;
            _Users = new List<User>();
            _Updater = new Utility.Updater();
            _Register = new AgentCommandRegister(Command);
        }        

        public User CreateUser()
        {            
            var agent = _AgentProvider.Spawn();
            var rectifier = new AgentEventRectifier(_WatchTypes, agent);
            var user = new User(agent , rectifier);
            _Users.Add(user);
            // todo _Updater.Add(user.Agent);
            foreach(var g in user.Ghosts)
            {
                _Register.Regist(g.Item1,g.Item2);
            }
            rectifier.SupplyEvent += _Register.Regist;
            rectifier.UnsupplyEvent += (type , obj) => { _Register.Unregist(obj); };

            return user;
        }
        public void DestroyUser(string id)
        {
            var user = _Users.FirstOrDefault((u) => u.Id == id);
            if (user == null)
                return;
            foreach (var g in user.Ghosts)
            {
                _Register.Unregist(g.Item2);
            }
            _Users.RemoveAll(u => u.Id == id);
            // todo _Updater.Remove(user.Agent);
            user.Dispose();
        }
        protected override void _Launch()
        {
            
        }

        protected override void _Shutdown()
        {
            _Updater.Shutdown();
        }

        protected override void _Update()
        {
            _Updater.Working();
        }

        
    }
}