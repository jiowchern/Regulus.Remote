namespace Regulus.BehaviourTree
{
    public interface IAction : ITicker
    {
        void Start();
        void End();
    }


    
    
}