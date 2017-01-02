using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.CozDataStructures
{
    [CozFunction]
    public class CozPoint
    {
        public CozPoint()
        {
        }

        [CozParameter(0)]
        public float X
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float Y
        {
            get;
            set;
        }
    }
}
