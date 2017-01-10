using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CozmoAPI.EventObjects
{
    [DebuggerDisplay("Robot Event=( PostFrameID={PoseFrameID} PostOriginID={PostOriginID} {Pose} LeftWheelSpeedMMPS={LeftWheelSpeedMMPS}\r\n\tRightWheelSpeedMMPS={RightWheelSpeedMMPS} HeadAngle={HeadAngle} LiftAngle={LiftAngle} {Acceleration} Gyro{Gyro} BatteryVoltage={BatteryVoltage} Status={Status}")]
    [CozFunction(0x26, IsEvent=true)]
    public class RobotState
    {
        public RobotState()
        {
        }

        [CozParameter(0)]
        public int Timestamp
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int PoseFrameID
        {
            get;
            set;
        }

        [CozParameter(2)]
        public int PostOriginID
        {
            get;
            set;
        }

        [CozParameter(3)]
        public RobotPose Pose
        {
            get;
            set;
        }

        [CozParameter(4)]
        public float LeftWheelSpeedMMPS
        {
            get;
            set;
        }

        [CozParameter(5)]
        public float RightWheelSpeedMMPS
        {
            get;
            set;
        }

        [CozParameter(6)]
        public float HeadAngle
        {
            get;
            set;
        }

        [CozParameter(7)]
        public float LiftAngle
        {
            get;
            set;
        }

        [CozParameter(8)]
        public AccelData Acceleration
        {
            get;
            set;
        }

        [CozParameter(9)]
        public AccelData Gyro
        {
            get;
            set;
        }

        [CozParameter(10)]
        public float BatteryVoltage
        {
            get;
            set;
        }

        [CozParameter(11)]
        public int Status
        {
            get;
            set;
        }

        [CozParameter(12)]
        public short LastPathID
        {
            get;
            set;
        }

        [CozParameter(13)]
        public short CliffDataRaw
        {
            get;
            set;
        }

        [CozParameter(14)]
        public byte CurrentPathSegment
        {
            get;
            set;
        }

        [CozParameter(15)]
        public byte NumberOfFreeSegmentSlots
        {
            get;
            set;
        }
        
    }

    [CozFunction]
    [DebuggerDisplay("Acceleration=( X={X}mmps2 Y={Y}mmps2 Z={Z}mmps2 )")]
    public class AccelData
    {
        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }
    }

    [CozFunction]
    [DebuggerDisplay("RobotPose=( X={X} Y={Y} Z={Z} AngleRad={AngleRad} PitchAngleRad={PitchAngleRad} )")]
    public class RobotPose
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
        public float AngleRad
        {
            get;
            set;
        }

        [CozParameter(4)]
        public float PitchAngleRad
        {
            get;
            set;
        }

    }
}
