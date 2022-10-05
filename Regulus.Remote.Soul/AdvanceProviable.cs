namespace Regulus.Remote.Soul
{
    public interface AdvanceProviable
    {
        event System.Action<Advanceable> JoinEvent;
        event System.Action<Advanceable> LeaveEvent;
    }
}
