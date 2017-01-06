using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.PlaceOnObject)]
    public class ActionPlaceOnObject : IRobotActionUnion
    {
        public ActionPlaceOnObject()
        {
            MotionProfile = new CozPathMotionProfile();
            UsePreDockPost = true;
            CheckForObjectOnTop = true;
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
        public float ApproachingAngle
        {
            get;
            set;
        }

        [CozParameter(3)]
        public bool UsePreDockPost
        {
            get;
            set;
        }

        [CozParameter(4)]
        public bool UseManualSpeed
        {
            get;
            set;
        }

        [CozParameter(5)]
        public bool CheckForObjectOnTop
        {
            get;
            set;
        }
    }
}
