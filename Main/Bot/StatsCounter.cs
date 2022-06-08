using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main
{
    public class StatsCounter
    {
        private readonly long m_RetentionInterval;
        private List<long> m_data;
        private readonly object m_lockData;
        private long m_counterCache;
        private long m_counterLastChecked;
        private long m_counterCacheTime;

        public StatsCounter(TimeSpan retentionInterval)
        {
            m_RetentionInterval = retentionInterval.Ticks;
            m_counterCacheTime = TimeSpan.TicksPerMinute;
            m_counterCache = 0;
            m_counterLastChecked = DateTime.MinValue.Ticks;
            m_lockData = new object();
            m_data = new List<long>();
        }

        public void IncrementCounter()
        {
            lock (m_lockData)
            {
                m_data.Add(DateTime.Now.Ticks);
                m_counterCache = m_data.Count();
            }
        }

        public void Add(DateTime dt)
        {
            lock (m_lockData)
            {
                m_data.Add(dt.Ticks);
                m_counterCache = m_data.Count();
            }
        }

        public long GetCount()
        {
            long now = DateTime.Now.Ticks;
            long cachediff = now - m_counterCacheTime;
            if (cachediff > m_counterLastChecked)
            {
                long removeLimit = now - m_RetentionInterval;
                lock (m_lockData)
                {
                    m_data.RemoveAll(l => l < removeLimit);
                    m_counterCache = m_data.Count();
                    m_counterLastChecked = now;
                }
            }
            return m_counterCache;
        }
    }
}