using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.DriveWheels)]
    public class ActionDriveWheels : IRobotActionUnion
    {
        public ActionDriveWheels()
        {
        }

        [CozParameter(0)]
        public float LeftWheelMMPS
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float RightWheelMMPS
        {
            get;
            set;
        }

        [CozParameter(2)]
        public float LeftWheelAccelMMPS2
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float RightWheelAccelMMPS2
        {
            get;
            set;
        }
    }
}
