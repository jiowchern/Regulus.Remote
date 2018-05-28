using System.Collections;
using System.Collections.Generic;

namespace Regulus.Lockstep
{
    public class Driver<TCommand>
    {
        
        private readonly int _KeyFrame;
        private int _FrameCount;
        private readonly Propeller _Propeller;
        private readonly System.Collections.Generic.Queue<TCommand> _Commands;
        public Driver(long frame_interval ,int key_frame)
        {        
            _KeyFrame = key_frame;
            _Commands = new Queue<TCommand>();            
            _Propeller = new Propeller(frame_interval);
        }

        public void Push(TCommand command)
        {
            for (int i = 0; i < _KeyFrame; i++)
            {
                _Propeller.Heartbeat();
            }            
            _Commands.Enqueue(command);
        }

        public void Drive(long delta , out bool frame , out TCommand command  )
        {
            frame = false;
            command = default(TCommand);
            var ticks = delta;            
            if (_Propeller.Propel(ticks))
            {
                _FrameCount++;
                if (_FrameCount == _KeyFrame)
                {
                    _FrameCount = 0;
                    command = _Commands.Dequeue();                    
                }
                frame = true;
            }            
        }
    }
}