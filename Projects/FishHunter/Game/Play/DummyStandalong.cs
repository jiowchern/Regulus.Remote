namespace VGame.Project.FishHunter.Play
{
    public class DummyStandalong : Regulus.Remoting.ICore
    {

        VGame.Project.FishHunter.Play.Center _Center;
        VGame.Project.FishHunter.DummyFrature _Storage;

        Regulus.Remoting.ICore _Core { get { return _Center; } }
        Regulus.Utility.CenterOfUpdateable _Updater;
        public DummyStandalong()
        {
            _Storage = new DummyFrature();
            _Updater = new Regulus.Utility.CenterOfUpdateable();
            _Center = new Center(_Storage , _Storage , _Storage );
        }

        void Regulus.Remoting.ICore.ObtainBinder(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.ObtainBinder(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Updater.Add(_Center);
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }
    }

}
