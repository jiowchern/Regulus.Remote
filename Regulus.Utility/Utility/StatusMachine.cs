using Regulus.Collection;

namespace Regulus.Utility
{
    public class EmptyStage : IStatus
	{
		void IStatus.Enter()
		{
		}

		void IStatus.Leave()
		{
		}

		void IStatus.Update()
		{
		}
	}
	public class StatusMachine
	{
		private readonly Queue<IStatus> _StandBys;

		public IStatus Current { get; private set; }

		public StatusMachine()
		{
			_StandBys = new Queue<IStatus>();
		}

		public void Push(IStatus new_stage)
		{
			_StandBys.Enqueue(new_stage);
		}

		public bool Update()
		{
			_SetCurrent();
			_UpdateCurrent();

			return Current != null;
		}

		private void _SetCurrent()
		{
			IStatus stage;
			if(_StandBys.TryDequeue(out stage))
			{
				if(Current != null)
				{
					Current.Leave();
				}

				stage.Enter();
				Current = stage;
			}
		}

		private void _UpdateCurrent()
		{
			if(Current != null)
			{
				Current.Update();
			}
		}

		public void Termination()
		{
			_StandBys.DequeueAll();
			if(Current != null)
			{
				Current.Leave();
				Current = null;
			}
		}

		public void Empty()
		{
			Push(new EmptyStage());
		}
	}



    public class StatusMachine<TArg> 
    {
        private readonly Queue<IStatus<TArg>> _StandBys;

        public IStatus<TArg> Current { get; private set; }

        public StatusMachine()
        {
            _StandBys = new Queue<IStatus<TArg>>();
        }

        public void Push(IStatus<TArg> new_stage)
        {
            _StandBys.Enqueue(new_stage);
        }

        public bool Update(TArg args)
        {
            
            _SetCurrent();
            _UpdateCurrent(args);

            return Current != null;
        }

        private void _SetCurrent()
        {
            IStatus<TArg> stage;
            if (_StandBys.TryDequeue(out stage))
            {
                if (Current != null)
                {
                    Current.Leave();
                }

                stage.Enter();
                Current = stage;
            }
        }

        private void _UpdateCurrent(TArg arg)  
        {
            if (Current != null)
            {
                Current.Update(arg);
                
            }
        }

        public void Termination()
        {
            _StandBys.DequeueAll();
            if (Current != null)
            {
                Current.Leave();
                Current = default(IStatus<TArg>);
            }
        }

        public void Empty()
        {
            Push(new EmptyStage<TArg>());
        }
    }

    internal class EmptyStage<TArg> : IStatus<TArg>
    {
        void IStatus<TArg>.Enter()
        {
            
        }

        void IStatus<TArg>.Leave()
        {
            
        }

        void IStatus<TArg>.Update(TArg obj)
        {
            
        }
    }
}
