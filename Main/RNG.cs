using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main
{
    public static class RNG
    {
        private static Random m_rand;
        private static int m_count;

        public static int Next(int min, int max)
        {
            if (m_count < 1)
            {
                m_rand = new Random();
                m_count = 100000;
            }
            m_count--;

            return m_rand.Next(min, max + 1);
            
        }
    }
}
