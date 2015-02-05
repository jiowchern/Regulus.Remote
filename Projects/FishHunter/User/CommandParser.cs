using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

namespace VGame.Project.FishHunter
{
    using Regulus.Extension;
    public class CommandParser : Regulus.Framework.ICommandParsable<IUser>
    {
    
        
        private Regulus.Utility.Command _Command;
        private Regulus.Utility.Console.IViewer _View;
        private IUser _User;

        public CommandParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {            
            this._Command = command;
            this._View = view;
            this._User = user;            
        }
        void Regulus.Framework.ICommandParsable<IUser>.Clear()
        {
            
        }
        
        private void _ConnectResult(bool result)
        {
            _View.WriteLine(string.Format("Connect result {0}", result));

        }

        void Regulus.Framework.ICommandParsable<IUser>.Setup(Regulus.Remoting.IGPIBinderFactory factory)
        {            
            var connect = factory.Create<Regulus.Utility.IConnect>(_User.Remoting.ConnectProvider);
            connect.Bind("Connect[result , ipaddr ,port]", (gpi) => { return new Regulus.Remoting.CommandParamBuilder().BuildRemoting<string, int, bool>(gpi.Connect, _ConnectResult); });


            var online = factory.Create<Regulus.Utility.IOnline>(_User.Remoting.OnlineProvider);
            online.Bind("Disconnect", (gpi) => { return new Regulus.Remoting.CommandParamBuilder().Build(gpi.Disconnect); });
            online.Bind("Ping", (gpi) => { return new Regulus.Remoting.CommandParamBuilder().Build(() => { _View.WriteLine( "Ping : " + gpi.Ping.ToString() ); }); });


            var verify = factory.Create<VGame.Project.FishHunter.IVerify>(_User.VerifyProvider);

            verify.Bind("Login[result,id,password]", (gpi) => { return new Regulus.Remoting.CommandParamBuilder().BuildRemoting<string, string, bool>(gpi.Login, _VerifyResult); });
        }

        private void _VerifyResult(bool result)
        {
            _View.WriteLine(string.Format("Verify result {0}", result));
        }

        
    }
}
