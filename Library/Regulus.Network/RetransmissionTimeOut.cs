namespace Regulus.Network.RUDP
{
    internal class RetransmissionTimeOut
    {
        private long _RTTVAL;
        private long _SRTT;
        private long  _RTT;
        public long Value { get; set; }
        public long RTT { get { return _SRTT; } }

        public RetransmissionTimeOut()
        {
            _RTTVAL = (long)(Timestamp.OneSecondTicks * 0.05);
            _SRTT = (long)(Timestamp.OneSecondTicks * 0.1);
            _RTT = (long)(Timestamp.OneSecondTicks * 0.1);

            Update(_RTT, 0);
        }

        public void Update(long rtt,long delta)
        {
            _SRTT = (long)(0.875 * _RTT + 0.125 * rtt);
            _RTT = rtt;
            _RTTVAL = (long)(0.75 * _RTTVAL + 0.25 * _Abs(_SRTT - rtt));            
            Value = _SRTT + _Max(delta, 4 * _RTTVAL);
        }

        private long _Abs(long val)
        {
            return val < 0 ? 0 - val : val;
        }

        private long _Max(long time_delta_ticks, long rttval)
        {
            return time_delta_ticks > rttval ? time_delta_ticks : rttval;
        }
    }
}