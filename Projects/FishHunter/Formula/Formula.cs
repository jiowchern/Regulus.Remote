using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Formula
{
    public class Server : Regulus.Utility.ICore
    {
        bool _Enable;
        VGame.Project.FishHunter.Storage.Proxy _Storage;
        Storage.IUser _StorageUser;
        Regulus.Utility.Updater _Updater;
        Regulus.Utility.StageMachine _Machine;
        private string _Account;
        private string _Password;
        private string _IpAddress;
        private int _Port;

        Regulus.Collection.Queue<Regulus.Remoting.ISoulBinder> _Binders;

        public Server()
        {            
            _Setup();
        }

        private void _Setup()
        {
            Regulus.Utility.Ini config = new Regulus.Utility.Ini(_ReadConfig());

            _IpAddress = config.Read("Storage", "ipaddr");
            _Port = int.Parse(config.Read("Storage", "port"));
            _Account = config.Read("Storage", "account");
            _Password = config.Read("Storage", "password");


            _Storage = new Storage.Proxy();
            _Machine = new Regulus.Utility.StageMachine();
            _Updater = new Regulus.Utility.Updater();
            _Binders = new Regulus.Collection.Queue<Regulus.Remoting.ISoulBinder>();
            _Enable = true;
        }

        private string _ReadConfig()
        {
            return System.IO.File.ReadAllText("config.ini");
        }


        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Binders.Enqueue(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            _Machine.Update();
            return _Enable;
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
                        
            _Updater.Shutdown();
            _Machine.Termination();
        }

        void Regulus.Framework.ILaunched.Launch()
        {            
            _Updater.Add(_Storage);
            _ToConnectStorage(_Storage.SpawnUser("user"));            
        }

        private void _ToConnectStorage(Storage.IUser user)
        {
            _StorageUser = user;
            var stage = new VGame.Project.FishHunter.ConnectStorageStage(user, _IpAddress, _Port);
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
            var stage = new VGame.Project.FishHunter.VerifyStorageStage(user , _Account , _Password);
            stage.DoneEvent += _VerifyResult;
            _Machine.Push(stage);
        }

        private void _VerifyResult(bool verify_result)
        {

            if (verify_result)
            {
                _ToBuildStorageController();
                
            }
            else
                throw new SystemException("stroage verify fail");
        }
        
        private void _ToBuildStorageController()
        {
            var stage = new VGame.Project.FishHunter.BuildStorageControllerStage(_StorageUser);
            stage.DoneEvent += _ToRunFormula;
            _Machine.Push(stage);
        }

        private void _ToRunFormula(StorageController controller)
        {
            var stage = new VGame.Project.FishHunter.RunFormulaStage(controller, _Binders);
            stage.DoneEvent += _ToShutdown;
            _Machine.Push(stage);
        }

        void _ToShutdown()
        {
            _Enable = false;
        }

        
    }
}
