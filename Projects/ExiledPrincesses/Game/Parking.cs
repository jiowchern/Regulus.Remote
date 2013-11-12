using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    class Parking : Regulus.Game.IStage, Regulus.Project.ExiledPrincesses.IParking
    {
        public delegate void OnSelectCompiled(ActorInfomation actor_infomation);
        public event OnSelectCompiled SelectCompiledEvent;

        public delegate void OnVerify();
        public event OnVerify VerifyEvent;

        private AccountInfomation _AccountInfomation;
        private Remoting.ISoulBinder _Binder;
        

        public Parking(Remoting.ISoulBinder binder, AccountInfomation account_infomation)
        {            
            this._Binder = binder;
            
            this._AccountInfomation = account_infomation;
            
        }
        void Regulus.Game.IStage.Enter()
        {
            _Binder.Bind<Regulus.Project.ExiledPrincesses.IParking>(this);
        }

        void Regulus.Game.IStage.Leave()
        {
            _Binder.Unbind<Regulus.Project.ExiledPrincesses.IParking>(this);
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
        

        Remoting.Value<ActorInfomation> IParking.SelectActor(string name)
        {
            var a = new ActorInfomation() { Id = Guid.NewGuid(), Name = name };
            SelectCompiledEvent(a);
            return a;
        }
    }
}
