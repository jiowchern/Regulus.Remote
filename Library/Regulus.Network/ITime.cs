using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Network
{
    public interface ITime
    {
        long OneSeconds { get; }
        long Now { get; }
        long Delta { get; }
        void Sample();
    }

    public class Time : ITime
    {
        readonly long _OneSeconds;
        private long _Delta;
        private long _Now;
        

        public Time()
        {
            _OneSeconds = System.TimeSpan.FromSeconds(1).Ticks;
            _Now = System.DateTime.Now.Ticks;
           
        }
        long ITime.OneSeconds
        {
            get { return _OneSeconds; }
        }

        long ITime.Now
        {
            get { return _Now; }
        }

        long ITime.Delta
        {
            get { return _Delta; }
        }

        void ITime.Sample()
        {
            var now = System.DateTime.Now.Ticks;
            _Delta = now - _Now;
            _Now = now;
        }
    }
}
