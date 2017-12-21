using System;

namespace Regulus.Game
{
    public interface IComponment
    {
        void Start(Entity entity);

        void End();

        void Update();
    }
}