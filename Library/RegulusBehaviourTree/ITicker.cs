namespace Regulus.BehaviourTree
{
    public interface ITicker
    {
        void Reset();
        TICKRESULT Tick(float delta);
    }
}