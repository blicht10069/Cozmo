using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.gotoObject)]
    public class ActionGotoObject : IRobotActionUnion
    {
        public ActionGotoObject()
        {
            MotionProfile = new CozPathMotionProfile();
            DistanceFromObjectMM = 0;
            UseManualSpeed = false;
            UsePreDockPose = false;
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
        public float DistanceFromObjectMM
        {
            get;
            set;
        }

        [CozParameter(3)]
        public bool UseManualSpeed
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
    }
}
