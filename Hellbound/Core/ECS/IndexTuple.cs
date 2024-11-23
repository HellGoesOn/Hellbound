using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public struct IndexTuple
    {
        public int startIndex;
        public int endIndex;

        public IndexTuple(int startIndex, int endIndex)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        public override string ToString()
        {
            return $"{startIndex} {endIndex}";
        }

        public static bool TryParse(string input, out IndexTuple? indexTuple)
        {
            string[] split = Regex.Replace(input, "[^0-9 ]", "").Split(" ");
            if(split.Length < 2 || !int.TryParse(split[0], out int a) || !int.TryParse(split[1], out int b))
            {
                indexTuple = null;
                return false;
            }
            indexTuple = new IndexTuple(a, b);
            return true;
        }
    }
}
