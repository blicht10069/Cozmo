using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.PlaceRelObject)]
    public class ActionPlaceRelObject : IRobotActionUnion
    {
        public ActionPlaceRelObject()
        {
            MotionProfile = new CozPathMotionProfile();
            UsePreDockPose = false;
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
        public float PlacementOffsetXMM
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float ApproachingAngleRadians
        {
            get;
            set;
        }

        [CozParameter(4)]
        public bool UseApproachingAngle
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

    }
}
