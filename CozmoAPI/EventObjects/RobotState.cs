using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;

namespace CozmoAPI.EventObjects
{
    [DebuggerDisplay("Robot Event=( PostFrameID={PoseFrameID} PostOriginID={PostOriginID} {Pose} LeftWheelSpeedMMPS={LeftWheelSpeedMMPS}\r\n\tRightWheelSpeedMMPS={RightWheelSpeedMMPS} HeadAngle={HeadAngle} LiftAngle={LiftAngle} {Acceleration} Gyro{Gyro} BatteryVoltage={BatteryVoltage} Status={Status}")]
    [CozFunction(0x26, IsEvent = true)]
    public class RobotState
    {
        public static readonly RobotState Empty = new RobotState();
        public RobotState()
        {
            Pose = new CozPose3D();
            Acceleration = new AccelData();
            Gyro = new AccelData();
        }

        [CozParameter(0)]
        public CozPose3D Pose
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float AngleRad
        {
            get;
            set;
        }

        [CozParameter(2)]
        public float PitchRad
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float LeftWheelMMPS
        {
            get;
            set;
        }

        [CozParameter(4)]
        public float RightWheelMMPS
        {
            get;
            set;
        }

        [CozParameter(5)]
        public float HeadAngleRad
        {
            get;
            set;
        }

        [CozParameter(6)]
        public float LiftHeightMM
        {
            get;
            set;
        }

        [CozParameter(7)]
        public float BatteryVoltage
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
        public int CarryingObjectID
        {
            get;
            set;
        }

        [CozParameter(11)]
        public int CarryingObjectOnTopID
        {
            get;
            set;
        }

        [CozParameter(12)]
        public int HeadTrackingObjectID
        {
            get;
            set;
        }

        [CozParameter(13)]
        public int LocalizedToObjectID
        {
            get;
            set;
        }

        [CozParameter(14)]
        public int LastImageTimestamp
        {
            get;
            set;
        }

        [CozParameter(15)]
        public int Status
        {
            get;
            set;
        }

        [CozParameter(16)]
        public byte GameStatus
        {
            get;
            set;
        }

        [CozParameter(17)]
        public byte RobotID
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
