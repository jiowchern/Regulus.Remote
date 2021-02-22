namespace Regulus.Remote.Standalone.Test
{
    internal class GhostIGpiA : IGpiA, IGhost
    {
        private readonly long _Id;

        public GhostIGpiA(long id, bool ret_type)
        {
            this._Id = id;
        }
        event CallMethodCallback IGhost.CallMethodEvent
        {
            add
            {

            }

            remove
            {

            }
        }

        event EventNotifyCallback IGhost.AddEventEvent
        {
            add
            {

            }

            remove
            {

            }
        }

        event EventNotifyCallback IGhost.RemoveEventEvent
        {
            add
            {

            }

            remove
            {

            }
        }

        

        long IGhost.GetID()
        {
            return _Id;
        }

        object IGhost.GetInstance()
        {
            return this;
        }

        bool IGhost.IsReturnType()
        {
            return false;
        }
    }
}