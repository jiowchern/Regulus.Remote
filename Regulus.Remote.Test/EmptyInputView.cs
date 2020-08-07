using Regulus.Utility;

namespace RemotingTest
{
    internal class EmptyInputView : Console.IInput, Console.IViewer
    {
        event Console.OnOutput Console.IInput.OutputEvent
        {
            add { }
            remove { }
        }

        void Console.IViewer.WriteLine(string message)
        {
        }

        void Console.IViewer.Write(string message)
        {
        }
    }
}
