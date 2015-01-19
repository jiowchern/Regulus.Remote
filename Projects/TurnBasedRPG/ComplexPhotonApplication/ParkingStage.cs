using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class ParkingStage : Regulus.Utility.IStage<User>
    {
        Regulus.Project.TurnBasedRPG.Parking _Parking;
        Regulus.Utility.StageLock Regulus.Utility.IStage<User>.Enter(User obj)
        {
            _Parking = new Regulus.Project.TurnBasedRPG.Parking(obj.Id);
            _Parking.BackEvent += obj.Logout;
            _Parking.SelectEvent += obj.EnterWorld;
            obj.Provider.Bind<IParking>(_Parking);

            return null;
        }

        void Regulus.Utility.IStage<User>.Leave(User obj)
        {
            obj.Provider.Unbind<IParking>(_Parking);
        }

        void Regulus.Utility.IStage<User>.Update(User obj)
        {
            
        }
    }
}
