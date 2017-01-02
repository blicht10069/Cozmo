using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.setLiftHeight)]
    public class ActionSetLiftHeight : IRobotActionUnion
    {
        public ActionSetLiftHeight()
        {
            DurationSeconds = 0.3f;
        }

        [CozParameter(0)]
        public float HeightMM
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
