namespace Regulus.BehaviourTree
{
    public interface ITicker
    {
        TICKRESULT Tick(float delta);
    }
}