﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class ParkingStage : Samebest.Game.IStage<User>
    {
        Regulus.Project.TurnBasedRPG.Parking _Parking;
        void Samebest.Game.IStage<User>.Enter(User obj)
        {
            _Parking = new Regulus.Project.TurnBasedRPG.Parking(obj.Id);
            _Parking.BackEvent += obj.Logout;
            _Parking.SelectEvent += obj.EnterWorld;
            obj.Provider.Bind<IParking>(_Parking);
        }

        void Samebest.Game.IStage<User>.Leave(User obj)
        {
            obj.Provider.Unbind<IParking>(_Parking);
        }

        void Samebest.Game.IStage<User>.Update(User obj)
        {
            
        }
    }
}
