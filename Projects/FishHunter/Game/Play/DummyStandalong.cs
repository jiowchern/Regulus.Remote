namespace VGame.Project.FishHunter.Play
{
    public class DummyStandalong : Regulus.Utility.ICore
    {

        VGame.Project.FishHunter.Play.Center _Center;
        VGame.Project.FishHunter.DummyFrature _Storage;

        Regulus.Utility.ICore _Core { get { return _Center; } }
        Regulus.Utility.CenterOfUpdateable _Updater;
        public DummyStandalong()
        {
            _Storage = new DummyFrature();
            _Updater = new Regulus.Utility.CenterOfUpdateable();
            _Center = new Center(_Storage , _Storage , _Storage );
        }

        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.ObtainController(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_Center);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }
    }

}
