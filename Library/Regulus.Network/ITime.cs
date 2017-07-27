using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Stopwatch _Stopwatch;

        public Time()
        {
            _Stopwatch = new Stopwatch();
            _OneSeconds = Stopwatch.Frequency;
            _Stopwatch.Start();

            _Now = _Stopwatch.ElapsedTicks;
           
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
            var now = _Stopwatch.ElapsedTicks;
            _Delta = now - _Now;
            _Now = now;
        }
    }
}
