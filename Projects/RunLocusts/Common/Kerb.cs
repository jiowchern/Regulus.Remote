using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    public interface IActorData
    {
        Guid Id { get; }
    }

    public class ActorData : IActorData
    {

        Guid IActorData.Id
        {
            get { return Guid.NewGuid(); }
        }
    }
    public interface IKerb
    {

    }
    
}
