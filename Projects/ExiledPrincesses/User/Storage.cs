using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Standalone
{
	class Storage : Regulus.Project.ExiledPrincesses.IStorage
	{
        
        public Storage()
        { 

        }
		Regulus.Remoting.Value<AccountInfomation> IStorage.FindAccountInfomation(string name)
		{
			return new AccountInfomation() { Id = Guid.Empty , Name = name , Password = "1" };
		}

		void IStorage.Add(AccountInfomation ai)
		{
			
		}

        int _Count;
        Regulus.Remoting.Value<Pet> IStorage.FindPet(Guid id)
        {
            var pet = new Pet() { Id = Guid.NewGuid(), Owner = id };
            pet.Energy = new Energy(7);
            Func<bool>[] energyIncs = new Func<bool>[]{pet.Energy.IncGreen , pet.Energy.IncRed , pet.Energy.IncYellow }; 
            /*for(int i = 0 ; i < 3 ; ++i)
            {
                energyIncs[Regulus.Utility.Random.Next(0, 3)]();
            }*/
            pet.Name = _Count++ % 2 == 0 ? "蝙蝠" : "甲蟲";
            return pet;
        }

        void IStorage.Add(Pet pet)
        {
            
        }
    }
}

