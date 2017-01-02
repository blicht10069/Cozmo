using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.CozDataStructures
{
    [CozFunction]
    public class CozPose3D
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
        public float Z
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float Q0
        {
            get;
            set;
        }

        [CozParameter(4)]
        public float Q1
        {
            get;
            set;
        }

        [CozParameter(5)]
        public float Q2
        {
            get;
            set;
        }

        [CozParameter(6)]
        public float Q3
        {
            get;
            set;
        }

        [CozParameter(7)]
        public int OriginID
        {
            get;
            set;
        }

    }
}
