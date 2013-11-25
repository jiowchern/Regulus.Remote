using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public interface IZone
    {
        Regulus.Remoting.Value<IMap> Query(string map);
    }

    

    public interface IMap : Regulus.Game.IFramework
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
            _Hall.Update();
            _Loopers.Update();
        }










        Remoting.Value<IMap> IZone.Query(string map)
        {
            throw new NotImplementedException();
        }
    }
}
