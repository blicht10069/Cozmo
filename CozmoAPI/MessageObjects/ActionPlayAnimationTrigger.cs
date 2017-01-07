using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.PlayAnimationTrigger)]
    public class ActionPlayAnimationTrigger : IRobotActionUnion
    {
        [CozParameter(2)]
        private int mTriggerType;

        public ActionPlayAnimationTrigger()
            : base()
        {
            RobotID = 0;
            NumberOfLoops = 1;
            TriggerType = CozAnimationTriggerType.AcknowledgeFaceInitPause;
        }

        [CozParameter(0)]
        public byte RobotID
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int NumberOfLoops
        {
            get;
            set;
        }

        public CozAnimationTriggerType TriggerType
        {
            get { return (CozAnimationTriggerType)mTriggerType; }
            set { mTriggerType = (int)value; }
        }

        [CozParameter(3)]
        public bool UseLiftSafe
        {
            get;
            set;
        }

        [CozParameter(4)]
        public bool IgnoreBodyTrack
        {
            get;
            set;
        }

        [CozParameter(5)]
        public bool IgnoreHeadTrack
        {
            get;
            set;
        }

        [CozParameter(6)]
        public bool IgnoreLiftTrack
        {
            get;
            set;
        }
    }
}
