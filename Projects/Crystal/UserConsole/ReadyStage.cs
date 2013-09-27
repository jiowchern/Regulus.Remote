using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
namespace UserConsole
{
    class ReadyStage : Regulus.Game.IStage<Framework>
    {
        private Regulus.Utility.Command _Command;
        private Regulus.Project.Crystal.IUser _User;
        private Regulus.Utility.Console.IViewer _Viewer;

        public ReadyStage(Regulus.Utility.Command command,Regulus.Utility.Console.IViewer viewer, Regulus.Project.Crystal.IUser user)
        {
            // TODO: Complete member initialization
            this._Command = command;
            this._User = user;
            _Viewer = viewer;

            _User.VerifyProvider.Supply += _AddVerify;
            _User.VerifyProvider.Supply -= _RemoveVerify;
        }

        private void _RemoveVerify(Regulus.Project.Crystal.IVerify obj)
        {
            _Command.Unregister("createaccount");
        }
        
        void _AddVerify(Regulus.Project.Crystal.IVerify obj)
        {
            _Command.RemotingRegister<string, string, bool>("createaccount", obj.CreateAccount, _CreateAccountResult);            
        }

        void _CreateAccountResult(bool value)
        {
            if (value == true)
            {
                _Viewer.WriteLine("帳號建立成功.");
            }
            else
            {
                _Viewer.WriteLine("帳號建立失敗.");
            }
        }
        

        Regulus.Game.StageLock Regulus.Game.IStage<Framework>.Enter(Framework obj)
        {
            return null;
        }

        void Regulus.Game.IStage<Framework>.Leave(Framework obj)
        {
            
        }

        void Regulus.Game.IStage<Framework>.Update(Framework obj)
        {
            
        }
    }
}
