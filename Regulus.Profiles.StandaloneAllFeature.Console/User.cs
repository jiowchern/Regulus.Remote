using Regulus.Profiles.StandaloneAllFeature.Protocols;
using Regulus.Remote;

namespace Regulus.Profiles.StandaloneAllFeature.Server
{
    class User : Regulus.Profiles.StandaloneAllFeature.Protocols.Featureable
    {
        public readonly IBinder Binder;
        private readonly ISoul _Soul;

        public User(IBinder binder)
        {
            Binder = binder;
            _Soul = Binder.Bind<Regulus.Profiles.StandaloneAllFeature.Protocols.Featureable>(this);
        }

        Value<string> Featureable.Inc(string value)
        {
            string reversed = new string(value.Reverse().ToArray());
            return new Value<string>(reversed);
        }
    }
}
