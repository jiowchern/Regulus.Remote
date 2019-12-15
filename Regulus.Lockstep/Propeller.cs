using System.Collections.Generic;

namespace Regulus.Lockstep
{
    public class Propeller<TStep>
    {
        
        private readonly long _Interval;
        private long _Ticks;
        
        private readonly int _IntervalPerKeyFrame;
        private readonly int _KeyFrameBuffer;
        private  int _FrameCount;

        private readonly Queue<TStep> _Steps;
        public Propeller(long interval , int interval_per_key_frame,int key_frame_buffer)
        {
            _Steps = new Queue<TStep>();
            _Interval = interval;
        
            _IntervalPerKeyFrame = interval_per_key_frame;
            _KeyFrameBuffer = key_frame_buffer;
        }

        public void Push(TStep step)
        {
            _Steps.Enqueue(step);
            _FrameCount += _IntervalPerKeyFrame;

            if (_Steps.Count > _KeyFrameBuffer)
            {
                _Ticks += _Interval * _IntervalPerKeyFrame * _KeyFrameBuffer;
            }
        }
        public bool Advance(long delta ,out TStep step)
        {
            step = default(TStep);
            if (_FrameCount == 0)
                return false;
            _Ticks += delta;
            if (_Ticks >= _Interval )
            {
                _Ticks -= _Interval;
                _FrameCount--;
                if (_FrameCount % _IntervalPerKeyFrame == 0)
                {                    
                    step = _Steps.Dequeue();                    
                }
                return true;
            }

            return false;
        }
    }
}