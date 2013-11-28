using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public interface IZone
    {
        Regulus.Remoting.Value<IMap> Create(string map);

        void Destory(IMap map);
    }

    

    public interface IMap : Regulus.Game.IFramework
    {
        event Action<string> ToMapEvent;
        event Action<string> ToToneEvent;

        void Initial(Contingent.FormationType formationType, ITeammate[] teammate);
        void Release();
    }

    public class LocalTime : Regulus.Utility.Singleton<Regulus.Remoting.Time>
    {                
    }

    public class Zone : IZone
    {
        Regulus.Project.ExiledPrincesses.Game.Hall _Hall;
        IStorage _Storage;
        
        Regulus.Game.FrameworkRoot _Loopers;
        
        public Zone(IStorage storage)
        {
            
            _Storage = storage;
            _Hall = new Hall();
            _Loopers = new Regulus.Game.FrameworkRoot();
            
        }

        public void Enter(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.CreateUser(binder, _Storage, this);
        }

        public void Update()
        {
            LocalTime.Instance.Update();
            _Hall.Update();
            _Loopers.Update();
        }

        Remoting.Value<IMap> IZone.Create(string map)
        {
            throw new NotImplementedException();
        }


        void IZone.Destory(IMap map)
        {
            throw new NotImplementedException();
        }
    }
}
