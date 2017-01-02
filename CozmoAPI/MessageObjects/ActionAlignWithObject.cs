using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.alignWithObject)]
    public class ActionAlignWithObject : IRobotActionUnion
    {
        public ActionAlignWithObject()
        {
            MotionProfile = new CozPathMotionProfile();
            ObjectID = 0;
            DistancefromMarkerMM = 0f;
            ApproachAngleRadians = 0f;
            UseApproachAngle = false;
            UsePreDockPose = false;
            UseManualSpeed = false;
            AlignmentType = CozAlignmentType.LIFT_FINGER;
        }

        [CozParameter(0)]
        public int ObjectID
        {
            get;
            set;
        }

        [CozParameter(1)]
        public CozPathMotionProfile MotionProfile
        {
            get;
            set;
        }

        [CozParameter(2)]
        public float DistancefromMarkerMM
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float ApproachAngleRadians
        {
            get;
            set;
        }

        [CozParameter(4)]
        public bool UseApproachAngle
        {
            get;
            set;
        }

        [CozParameter(5)]
        public bool UsePreDockPose
        {
            get;
            set;
        }

        [CozParameter(6)]
        public bool UseManualSpeed
        {
            get;
            set;
        }

        [CozParameter(7)]
        public CozAlignmentType AlignmentType
        {
            get;
            set;
        }
    }
}
