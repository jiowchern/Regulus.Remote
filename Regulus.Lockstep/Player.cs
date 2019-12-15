using System;
using System.Collections.Generic;

namespace Regulus.Lockstep
{
    public class Player<T> : IPlayer<T>

    {
        public readonly ICommandProvidable<T> Providable;
        private Queue<Step<Driver<T>.Record>> _Steps;
        public readonly int Id;

        public Player(int id,ICommandProvidable<T> providable, IEnumerable<Step<Driver<T>.Record>> records)
        {
            Id = id;
            Providable = providable;
            _Steps = new Queue<Step<Driver<T>.Record>>(records);

        
        }

        

        

        int IPlayer<T>.Id
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