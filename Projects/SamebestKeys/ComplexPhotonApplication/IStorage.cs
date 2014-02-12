using System;
namespace Regulus.Project.SamebestKeys
{
    public interface IStorage
    {
        void Add(global::Regulus.Project.SamebestKeys.Serializable.AccountInfomation ai);
        void Add(global::Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation ai);
        bool CheckActorName(string name);
        global::Regulus.Project.SamebestKeys.Serializable.AccountInfomation FindAccountInfomation(string name);
        global::Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation[] FindActor(Guid id);
        bool RemoveActor(Guid id, string name);
        void SaveActor(global::Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation actor);
    }
}
