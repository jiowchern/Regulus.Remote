using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public interface IMap
    {
        void Enter(IEntity entity);
        void Leave(IEntity entity);

        
    }

    
    

    public class Zone 
    {
        Regulus.Project.ExiledPrincesses.Game.Hall _Hall;
        IStorage _Storage;
        
        Regulus.Game.FrameworkRoot _Loopers;
        Map _Map;
        public Zone(IStorage storage)
        {
            _Storage = storage;
            _Hall = new Hall();
            
            _Map = new Map();

            _Loopers = new Regulus.Game.FrameworkRoot();
            
        }

        public void Enter(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.CreateUser(binder, _Storage, _Map);
        }

        public void Update()
        {
            _Hall.Update();
            _Loopers.Update();
        }

       

        
        



        
    }
}
