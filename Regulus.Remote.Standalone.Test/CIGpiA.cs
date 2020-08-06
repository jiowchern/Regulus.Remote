namespace Regulus.Remote.Standalone.Test
{
    internal class CIGpiA : IGpiA, IGhost
    {
        public CIGpiA(long id , bool ret_type)
        {

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

        event PropertyNotifierCallback IGhost.AddSupplyNoitfierEvent
        {
            add
            {
                
            }

            remove
            {
                
            }
        }

        event PropertyNotifierCallback IGhost.RemoveSupplyNoitfierEvent
        {
            add
            {
                
            }

            remove
            {
                
            }
        }

        event PropertyNotifierCallback IGhost.AddUnsupplyNoitfierEvent
        {
            add
            {
                
            }

            remove
            {
                
            }
        }

        event PropertyNotifierCallback IGhost.RemoveUnsupplyNoitfierEvent
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
            return 1;
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