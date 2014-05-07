using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class ParkingStage : Regulus.Game.IStage<User>
    {
        IStorage _Storage;
        Regulus.Project.SamebestKeys.Parking _Parking;
        public ParkingStage(IStorage storage)
        {
            _Storage = storage;
        }
        Regulus.Game.StageLock Regulus.Game.IStage<User>.Enter(User obj)
        {
            _Parking = new Regulus.Project.SamebestKeys.Parking(obj.Id, _Storage);
            _Parking.BackEvent += obj.Logout;
            _Parking.SelectEvent += obj.ToLevel;
            obj.Provider.Bind<IParking>(_Parking);

            return null;
        }

        void Regulus.Game.IStage<User>.Leave(User obj)
        {
            obj.Provider.Unbind<IParking>(_Parking);
        }

        void Regulus.Game.IStage<User>.Update(User obj)
        {
            
        }
    }
}
