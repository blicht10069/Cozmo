using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.CozDataStructures
{
    [CozFunction]
    public class CozRect
    {
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

        [CozParameter(2)]
        public float Width
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float Height
        {
            get;
            set;
        }
    }
}
