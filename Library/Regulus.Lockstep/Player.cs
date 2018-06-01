using System;
using System.Collections.Generic;

namespace Regulus.Lockstep
{
    public class Player<T> : IPlayer<T>

    {
        public readonly ICommandProvidable<T> Providable;
        private Queue<Step<Driver<T>.Record>> _Steps;
        public readonly Guid Id;

        public Player(ICommandProvidable<T> providable, IEnumerable<Step<Driver<T>.Record>> records)
        {
            Id = Guid.NewGuid();
            Providable = providable;
            _Steps = new Queue<Step<Driver<T>.Record>>(records);

        
        }

        

        

        Guid IPlayer<T>.Id
        {
            get { return Id; }
        }

        IEnumerable<Step<Driver<T>.Record>> IPlayer<T>.PopSteps()
        {
            return _Steps;
        }

        public void Push(Step<Driver<T>.Record> step)
        {
            _Steps.Enqueue(step);
        }
    }
}