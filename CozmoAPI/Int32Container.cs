using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI
{
    [CozFunction]
    public class Int32Container
    {
        public static Int32Container[] Create(int count, int value = 0)
        {
            Int32Container[] ret = new Int32Container[count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = new Int32Container() { Value = value };
            return ret;
        }

        [CozParameter(0)]
        public int Value
        {
            get;
            set;
        }
    }
}
