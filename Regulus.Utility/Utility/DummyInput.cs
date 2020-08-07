namespace Regulus.Utility
{
    public class DummyInput : Console.IInput
    {
        event Console.OnOutput Console.IInput.OutputEvent
        {
            add { }
            remove { }
        }
    }
}
