using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(33)]
    public class ActionTurnInPlace : IRobotActionUnion
    {        
        public ActionTurnInPlace()
            : base()
        {
            AngleInRad = ToRadians(90);
            SpeedRadPerSec = 1.57f;
            AccelerationRadPerSec2 = 0f;
            IsAbsolute = false;
            RobotId = 0;
        }

        public static float ToRadians(double angle)
        {
            return (float)(Math.PI / 180d * angle);
        }

        public static double ToDegrees(float radians)
        {
            return 180d / Math.PI * (double)radians;
        }

        [CozParameter(0)]
        public float AngleInRad
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float SpeedRadPerSec
        {
            get;
            set;
        }

        [CozParameter(2)]
        public float AccelerationRadPerSec2
        {
            get;
            set;
        }

        [CozParameter(3)]
        public bool IsAbsolute
        {
            get;
            set;
        }

        [CozParameter(4)]
        public byte RobotId
        {
            get;
            set;
        }
    }
}
