
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regulus.Extension;
using VGame.Project.FishHunter;

namespace VGame.Project.FishHunter.Play
{



    public class Complex : Regulus.Remoting.ICore
    {
        Regulus.Utility.StageMachine _Machine;
        Regulus.Utility.Updater _Updater;
        VGame.Project.FishHunter.Storage.Proxy _Storage;
        VGame.Project.FishHunter.Formula.Client _Formula;
        VGame.Project.FishHunter.Play.Center _Center;

        Regulus.CustomType.Verify _StorageVerifyData;
        Regulus.CustomType.Verify _FormulaVerifyData;
        
        Regulus.Remoting.ICore _Core { get { return _Center; } }
        Storage.IUser _StorageUser;
        Formula.IUser _FormulaUser;

        Regulus.Utility.LogFileRecorder _LogRecorder;
        public Complex()
        {
            _LogRecorder = new Regulus.Utility.LogFileRecorder("Play");            

            _StorageVerifyData = new Regulus.CustomType.Verify();
            _FormulaVerifyData = new Regulus.CustomType.Verify();
            _Machine = new Regulus.Utility.StageMachine();
            _Updater = new Regulus.Utility.Updater();

            _BuildParams();
            _BuildUser();
        }
        private void _BuildParams()
        {
            Regulus.Utility.Ini config = new Regulus.Utility.Ini(_ReadConfig());

            _StorageVerifyData.IPAddress = config.Read("Storage", "ipaddr");
            _StorageVerifyData.Port = int.Parse(config.Read("Storage", "port"));
            _StorageVerifyData.Account = config.Read("Storage", "account");
            _StorageVerifyData.Password = config.Read("Storage", "password");

            _FormulaVerifyData.IPAddress = config.Read("Formula", "ipaddr");
            _FormulaVerifyData.Port = int.Parse(config.Read("Formula", "port"));
            _FormulaVerifyData.Account = config.Read("Formula", "account");
            _FormulaVerifyData.Password = config.Read("Formula", "password");
        }
        private void _BuildUser()
        {
            if(_IsIpAddress(_FormulaVerifyData.IPAddress))
            {
                _Formula = new VGame.Project.FishHunter.Formula.Client();
                _Formula.Selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.RemotingUserFactory());
                _FormulaUser = _Formula.Selector.CreateUserProvider("remoting").Spawn("1");
                
            }
            else
            {
                var center = new VGame.Project.FishHunter.Formula.Center( new VGame.Project.FishHunter.Formula.StorageController( new DummyFrature() ));
                _Updater.Add(center);
                _Formula = new VGame.Project.FishHunter.Formula.Client();
                _Formula.Selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.StandalongUserFactory(center));                
                _FormulaUser = _Formula.Selector.CreateUserProvider("remoting").Spawn("1");
                
            }

            if (_IsIpAddress(_StorageVerifyData.IPAddress))
            {
                
                
                _Storage = new VGame.Project.FishHunter.Storage.Proxy(new VGame.Project.FishHunter.Storage.RemotingFactory());
                _StorageUser = _Storage.SpawnUser("user");
            }
            else
            {

                var center = new VGame.Project.FishHunter.Storage.Center( new DummyFrature() );
                _Updater.Add(center);
                var factory = new VGame.Project.FishHunter.Storage.StandalongFactory(center);
                _Storage = new VGame.Project.FishHunter.Storage.Proxy(factory);
                _StorageUser = _Storage.SpawnUser("user");
            }
            
        }

        private bool _IsIpAddress(string ip)
        {
            System.Net.IPAddress ipaddr;
            return System.Net.IPAddress.TryParse(ip, out ipaddr);
        }

        void Regulus.Remoting.ICore.ObtainBinder(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.ObtainBinder(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            _Machine.Update();
            return true;
        }
        void Regulus.Framework.IBootable.Shutdown()
        {            
            _Updater.Shutdown();
            Regulus.Utility.Log.Instance.RecordEvent -= _LogRecorder.Record;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        
        void Regulus.Framework.IBootable.Launch()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Regulus.Utility.Log.Instance.RecordEvent += _LogRecorder.Record;                                  
            _Updater.Add(_Storage);
            _Updater.Add(_Formula);
            _ToConnectStorage(_StorageUser);            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex =(Exception)e.ExceptionObject;
            _LogRecorder.Record(ex.ToString());
            _LogRecorder.Save();
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
            _Center = new Center(features.AccountFinder, features.FishStageQueryer, features.RecordQueriers);

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

        

        

        

        private string _ReadConfig()
        {
            return System.IO.File.ReadAllText("config.ini");
        }

        
    }
}
