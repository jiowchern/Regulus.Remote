namespace Regulus.Remoting.Unity
{
    public abstract class Assigner
    {
        public abstract void Register<T>(Adsorber<T> adsorber);

        public abstract void Unregister<T>(Adsorber<T> adsorber);

    }

}

