using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(27)]
    public class ActionSetHeadAngle : IRobotActionUnion
    {
        public const float DEF_SPEED = 0.01745f*10;

        public ActionSetHeadAngle()
            : base()
        {
            MaxSpeedRadPerS = DEF_SPEED;
            AccelRadPerS2 = 0;
            DurationSeconds = 0f;
        }

        [CozParameter(0)]
        public float AngleRad
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float MaxSpeedRadPerS
        {
            get;
            set;
        }

        [CozParameter(2)]
        public float AccelRadPerS2
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float DurationSeconds
        {
            get;
            set;
        }
    }
}
