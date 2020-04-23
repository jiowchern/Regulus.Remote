using Regulus.Remote;
using System;

namespace Regulus.Application.Client.Test
{
    public class CType : IType, IGhost
    {
        public readonly Guid Id;

        public CType(Guid id)
        {
            this.Id = id;
        }
        event CallMethodCallback IGhost.CallMethodEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        Guid IGhost.GetID()
        {
            return Id;
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