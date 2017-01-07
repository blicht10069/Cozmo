using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.DriveArc)]
    public class ActionDriveArc : IRobotActionUnion
    {
        public ActionDriveArc()
        {
            SpeedMMPS = 10f;
            RadiusMM = 10f;
        }

        [CozParameter(0)]
        public float SpeedMMPS
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float RadiusMM
        {
            get;
            set;
        }
    }
}
