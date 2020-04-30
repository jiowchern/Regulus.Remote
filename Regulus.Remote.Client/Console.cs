namespace Regulus.Remote.Client
{
    public class Console : Regulus.Utility.WindowConsole
    {
        public Console(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input) : base(view , input)
        {
            
        }
        protected override void _Launch()
        {
            
        }

        protected override void _Shutdown()
        {
            throw new System.NotImplementedException();
        }

        protected override void _Update()
        {
            throw new System.NotImplementedException();
        }

        
    }
}