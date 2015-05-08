
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regulus.Extension;
using VGame.Project.FishHunter;

namespace VGame.Project.FishHunter.Play
{



    class Complex : Regulus.Utility.ICore
    {
        Regulus.Utility.StageMachine _Machine;
        Regulus.Utility.Updater _Updater;
        VGame.Project.FishHunter.Storage.Proxy _Storage;
        VGame.Project.FishHunter.Formula.Client _Formula;
        VGame.Project.FishHunter.Play.Center _Center;

        Regulus.CustomType.Verify _StorageVerifyData;
        Regulus.CustomType.Verify _FormulaVerifyData;
        
        Regulus.Utility.ICore _Core { get { return _Center; } }
        Storage.IUser _StorageUser;
        Formula.IUser _FormulaUser;
        public Complex()
        {
            _StorageVerifyData = new Regulus.CustomType.Verify();
            _FormulaVerifyData = new Regulus.CustomType.Verify();
            _Machine = new Regulus.Utility.StageMachine();
            _Updater = new Regulus.Utility.Updater();
            _Storage = new VGame.Project.FishHunter.Storage.Proxy();
            _Formula = new VGame.Project.FishHunter.Formula.Client();

            _Formula.Selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.RemotingUserFactory());
            _FormulaUser = _Formula.Selector.CreateUserProvider("remoting").Spawn("1");
            _StorageUser = _Storage.SpawnUser("user");
        }

        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.ObtainController(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            
            return true;
        }
        void Regulus.Framework.ILaunched.Shutdown()
        {

            _Updater.Shutdown();
        }

        
        void Regulus.Framework.ILaunched.Launch()
        {            
            _BuildParams();            
            _Updater.Add(_Storage);
            _Updater.Add(_Formula);
            _ToConnectStorage(_StorageUser);            
        }

        private void _ToConnectStorage(Storage.IUser user)
        {            
            var stage = new VGame.Project.FishHunter.ConnectStorageStage(user, _StorageVerifyData.IPAddress , _StorageVerifyData.Port);
            stage.DoneEvent += _ConnectResult;
            _Machine.Push(stage);
        }

        private void _ConnectResult(bool result)
        {
            if (result)
            {
                _ToVerifyStorage(_StorageUser);
            }
            else
                throw new SystemException("stroage connect fail");

        }

        private void _ToVerifyStorage(Storage.IUser user)
        {
            var stage = new VGame.Project.FishHunter.VerifyStorageStage(user, _StorageVerifyData.Account, _StorageVerifyData.Password);
            stage.DoneEvent += _VerifyResult;
            _Machine.Push(stage);
        }

        private void _VerifyResult(bool verify_result)
        {

            if (verify_result)
            {
                _ToConnectFormula();
            }
            else
                throw new SystemException("stroage verify fail");
        }

        private void _ToConnectFormula()
        {

            var stage = new Regulus.Utility.ConnectStage(_FormulaUser.Remoting.ConnectProvider , _FormulaVerifyData.IPAddress , _FormulaVerifyData.Port );

            stage.SuccessEvent += _ToFormulaVerify;
            stage.FailEvent += _FormulaConnectFail;
            _Machine.Push(stage);
        }
        

        private void _ToFormulaVerify()
        {
            var stage = new VGame.Project.FishHunter.VerifyStage(_FormulaUser.VerifyProvider , _FormulaVerifyData.Account , _FormulaVerifyData.Password );

            stage.SuccessEvent += _ToBuildClient;
            stage.FailEvent += _FormulaVerifyFail;
            _Machine.Push(stage);
        }

        private void _ToBuildClient()
        {
            var stage = new VGame.Project.FishHunter.Play.BuildCenterStage(_FormulaUser, _StorageUser);

            stage.BuiledEvent += _Play;
            
            _Machine.Push(stage);
        }

        private void _Play(BuildCenterStage.ExternalFeature features)
        {
            _Center = new Center(features.AccountFinder, features.FishStageQueryer);

            _Updater.Add(_Center);
        }

       

        private void _FormulaVerifyFail()
        {
            throw new SystemException("formula verify fail");
        }

        private void _FormulaConnectFail()
        {
            throw new SystemException("formula connect fail");
        }

        

        

        private void _BuildParams()
        {
            Regulus.Utility.Ini config = new Regulus.Utility.Ini(_ReadConfig());

            _StorageVerifyData.IPAddress= config.Read("Storage", "ipaddr");
            _StorageVerifyData.Port = int.Parse(config.Read("Storage", "port"));
            _StorageVerifyData.Account = config.Read("Storage", "account");
            _StorageVerifyData.Password = config.Read("Storage", "password");

            _FormulaVerifyData.IPAddress = config.Read("formula", "ipaddr");
            _FormulaVerifyData.Port = int.Parse(config.Read("formula", "port"));
            _FormulaVerifyData.Account = config.Read("formula", "account");
            _FormulaVerifyData.Password = config.Read("formula", "password");            
        }

        private string _ReadConfig()
        {
            return System.IO.File.ReadAllText("config.ini");
        }

        
    }
}
