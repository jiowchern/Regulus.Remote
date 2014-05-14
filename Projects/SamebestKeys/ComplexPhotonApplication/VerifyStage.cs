using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class VerifyStage : Regulus.Game.IStage
    {
        Regulus.Project.SamebestKeys.Verify _Verify;
        User _User;
        IStorage _Storage;
        public VerifyStage(IStorage storage,User user)
        {
            _User = user;
            _Storage = storage;
            
        }
        void Regulus.Game.IStage.Enter()
        {
            _Verify = new Regulus.Project.SamebestKeys.Verify(_Storage);
            _Verify.LoginSuccess += _User.OnLoginSuccess;
            _Verify.QuitEvent += _User.Quit;

            _User.Provider.Bind<IVerify>(_Verify);
        }

        void Regulus.Game.IStage.Leave()
        {
            _User.Provider.Unbind<IVerify>(_Verify);
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
