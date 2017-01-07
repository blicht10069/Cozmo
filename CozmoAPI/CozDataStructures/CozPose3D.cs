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


        public double AngleZRad
        {
            get
            {
                return Math.Atan2(2 * (Q1 * Q2 + Q0 * Q3), 1 - 2 * (Math.Pow(Q2, 2) + Math.Pow(Q3, 2)));
            }
        }

        public double AngleZ
        {
            get { return Utilities.ToDegrees(AngleZRad); }
        }

        public CozPoint CalculateOffsetPosition(float radiusMM)
        {
            double angle = AngleZRad;
            float x = (float)(radiusMM * Math.Cos(angle));
            float y = (float)(radiusMM * Math.Sin(angle));
            return new CozPoint() { X = x + X, Y = y + Y };
        }

        public override string ToString()
        {
            return Utilities.DebugObject(this);
        }

    }
}
