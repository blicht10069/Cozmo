using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(11)]
    public class ActionMountCharger : IRobotActionUnion
    {
        public ActionMountCharger()
            : base()
        {
            ObjectID = 0;
            MotionProfile = new CozPathMotionProfile();
            UsePreDockPose = true;
            UseManualSpeed = false;
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
        public bool UsePreDockPose
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
    }
}
