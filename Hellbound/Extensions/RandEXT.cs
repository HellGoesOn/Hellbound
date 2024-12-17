using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Extensions
{
    public static class RandEXT
    {
        public static int Next(this Random rand, Array array) => rand.Next(array.Length);
    }
}
