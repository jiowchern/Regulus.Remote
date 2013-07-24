using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
	public interface IStage
	{
		void	Enter	();
		void	Leave	();
		void	Update	();
	}


    public class StageLock
    {
        public enum Status
        { 
            Locked,Unlock
        }
        public Status Current { get; private set; }
        public StageLock()
        {
            Current = Status.Locked;
        }

        public void Unlock()
        {
            Current = Status.Unlock;
        }
    }
	public interface IStage<T> 
	{
        StageLock Enter(T obj);
		void Leave(T obj);
		void Update(T obj);
	}
}
