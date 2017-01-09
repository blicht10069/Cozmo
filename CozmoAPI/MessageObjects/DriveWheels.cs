using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.DriveWheels)]
    public class DriveWheels 
    {
        public const float WHEEL_SPAN_MM = 40f;

        public DriveWheels()
        {
        }

        public static DriveWheels Configure(ArchType archType, float radius, float timeToTravelFullCircleInSeconds = 1f)
        {
            float circleSize = (float)(Math.PI * 2 * radius);
            float outsideWheel = circleSize / timeToTravelFullCircleInSeconds;
            float insideWheel = (float)((Math.PI * 2 * (radius - WHEEL_SPAN_MM)) / timeToTravelFullCircleInSeconds);
            DriveWheels ret = new DriveWheels();
            ret.LeftWheelMMPS = archType == ArchType.Clockwise ? outsideWheel : insideWheel;
            ret.RightWheelMMPS = archType == ArchType.Clockwise ? insideWheel : outsideWheel;
            ret.LeftWheelAccelMMPS2 = ret.LeftWheelMMPS;
            ret.RightWheelAccelMMPS2 = ret.RightWheelMMPS;
            return ret;
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

    public enum ArchType
    {
        Clockwise,
        CounterClockwise
    }
}
