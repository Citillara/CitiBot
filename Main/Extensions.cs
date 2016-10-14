using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main
{

    public static class Extensions
    {
        public static string[] Split(this string s, char c)
        {
            return s.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
