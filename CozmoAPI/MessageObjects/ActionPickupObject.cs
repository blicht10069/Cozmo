using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.pickupObject)]
    public class ActionPickupObject : IRobotActionUnion
    {
        public ActionPickupObject()
            : base()
        {
            MotionProfile = new CozPathMotionProfile();
            UsePreDockPose = true;
        }

        [CozParameter(0)]
        public int ObjectId
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
        public float ApproachAngleRad
        {
            get;
            set;
        }

        [CozParameter(3)]
        public bool UseApproachAngle
        {
            get;
            set;
        }

        [CozParameter(4)]
        public bool UsePreDockPose
        {
            get;
            set;
        }

        [CozParameter(5)]
        public bool UseManualSpeed
        {
            get;
            set;
        }
    }
}
