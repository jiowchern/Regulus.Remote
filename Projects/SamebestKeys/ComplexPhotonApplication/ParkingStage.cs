using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class ParkingStage : Regulus.Game.IStage
    {
        IStorage _Storage;
        User _User;
        Regulus.Project.SamebestKeys.Parking _Parking;
        public ParkingStage(IStorage storage , User user)
        {
            _User = user;
            _Storage = storage;
        }
        void Regulus.Game.IStage.Enter()
        {
            _Parking = new Regulus.Project.SamebestKeys.Parking(_User.Id, _Storage);
            _Parking.BackEvent += _User.ToLogout;            
            _Parking.SelectEvent += _User.ToFirst;
            _User.Provider.Bind<IParking>(_Parking);
            
        }

        void Regulus.Game.IStage.Leave()
        {
            _User.Provider.Unbind<IParking>(_Parking);
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
