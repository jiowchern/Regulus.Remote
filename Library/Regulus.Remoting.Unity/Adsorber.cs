

namespace Regulus.Remoting.Unity
{

    public interface Adsorber<T> 
    {
        

        void Supply(T gpi);


        void Unsupply(T gpi);

        T GetGPI();
    }


}