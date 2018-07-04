using System;

namespace Regulus.Game
{
    public interface IComponment
    {

        void Start();

        void End();
        void Update();
    }

    public interface IComponmentBootable : IComponment
    {
        void Start();

        void End();
    }

    public interface IComponmentUpdateable : IComponmentBootable
    {
        void Update();
    }


    
}