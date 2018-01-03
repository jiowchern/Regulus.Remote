using System.Diagnostics;

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
        private readonly long m_OneSeconds;
        private long m_Delta;
        private long m_Now;
        private readonly Stopwatch m_Stopwatch;

        public Time()
        {
            m_Stopwatch = new Stopwatch();
            m_OneSeconds = Stopwatch.Frequency;
            m_Stopwatch.Start();

            m_Now = m_Stopwatch.ElapsedTicks;
           
        }
        long ITime.OneSeconds {get { return m_OneSeconds; } } 

        long ITime.Now
        {
            get
            {
                lock (m_Stopwatch)
                {
                    return m_Now;
                }
                
            }
        }

        long ITime.Delta
        {
            get
            {
                lock (m_Stopwatch)
                {
                    return m_Delta;
                }
                
            }
        }

        void ITime.Sample()
        {
            lock (m_Stopwatch)
            {
                var now = m_Stopwatch.ElapsedTicks;
                m_Delta = now - m_Now;
                m_Now = now;
            }
            
        }
    }
}
