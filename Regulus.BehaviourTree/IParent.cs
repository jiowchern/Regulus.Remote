namespace Regulus.BehaviourTree
{
    internal interface IParent : ITicker 
    {
        void Add(ITicker ticker);
    }
}