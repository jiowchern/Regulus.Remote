using System;
using System.Collections.Generic;

namespace Regulus.Lockstep
{
    public interface IPlayer<TCommand>
    {        
        int Id { get; }

        IEnumerable<Step<Driver<TCommand>.Record>> PopSteps();
    }
}