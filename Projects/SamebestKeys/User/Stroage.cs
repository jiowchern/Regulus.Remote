using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys.Standalong
{
    class Stroage : Regulus.Project.SamebestKeys.IStorage
    {
        List<Project.SamebestKeys.Serializable.AccountInfomation> _AccountInfomations;
        List<Project.SamebestKeys.Serializable.DBEntityInfomation> _DBEntityInfomations;
        public Stroage()
        {
            _DBEntityInfomations = new List<Project.SamebestKeys.Serializable.DBEntityInfomation>();
            _AccountInfomations = new List<Project.SamebestKeys.Serializable.AccountInfomation>();

            var id = Guid.NewGuid();
            _AccountInfomations.Add(new Project.SamebestKeys.Serializable.AccountInfomation() { Id = id, Name = "1", Password = "1" });

            _DBEntityInfomations.Add(new Project.SamebestKeys.Serializable.DBEntityInfomation()
            {
                Look = new Project.SamebestKeys.Serializable.EntityLookInfomation() { Name = "1" } ,
                Owner = id,
                Property = new Project.SamebestKeys.Serializable.EntityPropertyInfomation()
            });
        }
        void Project.SamebestKeys.IStorage.Add(Project.SamebestKeys.Serializable.AccountInfomation ai)
        {
            _AccountInfomations.Add(ai);
        }

        void Project.SamebestKeys.IStorage.Add(Project.SamebestKeys.Serializable.DBEntityInfomation ai)
        {
            _DBEntityInfomations.Add(ai);
        }

        bool Project.SamebestKeys.IStorage.CheckActorName(string name)
        {
            var result = (from ei in _DBEntityInfomations where ei.Look.Name == name select ei).Count();
            return result != 0;
        }

        Project.SamebestKeys.Serializable.AccountInfomation Project.SamebestKeys.IStorage.FindAccountInfomation(string name)
        {

            return (from ai in _AccountInfomations where ai.Name == name select ai).FirstOrDefault();
        }

        Project.SamebestKeys.Serializable.DBEntityInfomation[] Project.SamebestKeys.IStorage.FindActor(Guid id)
        {

            return (from ei in _DBEntityInfomations where ei.Owner == id select ei).ToArray();
        }

        bool Project.SamebestKeys.IStorage.RemoveActor(Guid id, string name)
        {
            return _DBEntityInfomations.RemoveAll((ei) => { return ei.Owner == id && ei.Look.Name == name; }) != 0;
        }

        void Project.SamebestKeys.IStorage.SaveActor(Project.SamebestKeys.Serializable.DBEntityInfomation actor)
        {
            _DBEntityInfomations.Add(actor);
        }


        void Project.SamebestKeys.IStorage.CreateConsumptionPlayer(Guid owner)
        {
            
        }

        int Project.SamebestKeys.IStorage.QueryConsumptionCoins(Guid owner)
        {
            return 100;
        }

        void Project.SamebestKeys.IStorage.AddConsumptionCoins(Guid owner, int coins)
        {
            
        }
    }
    
}
