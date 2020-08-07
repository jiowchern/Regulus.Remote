namespace Regulus.Network
{
    internal class RetransmissionTimeOut
    {
        private long m_Rttval;
        private long m_Srtt;
        private long m_Rtt;
        public long Value { get; set; }
        public long Rtt { get { return m_Srtt; } }

        public RetransmissionTimeOut()
        {
            Reset();

        }

        public void Update(long Rtt, long Delta)
        {
            m_Srtt = (long)(0.875 * m_Rtt + 0.125 * Rtt);
            m_Rtt = Rtt;
            m_Rttval = (long)(0.75 * m_Rttval + 0.25 * Abs(m_Srtt - Rtt));
            Value = m_Srtt + Max(Delta, 4 * m_Rttval);
        }

        public void Reset()
        {
            m_Rttval = (long)Timestamp.OneSecondTicks;
            m_Srtt = (long)Timestamp.OneSecondTicks;
            m_Rtt = (long)Timestamp.OneSecondTicks;

            Update(m_Rtt, Delta: 0);
        }

        private long Abs(long Val)
        {
            return Val < 0 ? 0 - Val : Val;
        }

        private long Max(long TimeDeltaTicks, long Rttval)
        {
            return TimeDeltaTicks > Rttval ? TimeDeltaTicks : Rttval;
        }
    }
}