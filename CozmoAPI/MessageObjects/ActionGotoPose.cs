using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.gotoPose)]
    public class ActionGotoPose : IRobotActionUnion
    {
        public ActionGotoPose()
        {
            X = 0f;
            Y = 0f;
            AngleInRadians = 0f;
            MotionProfile = new CozPathMotionProfile();
            Level = 0;
            UseManualSpeed = false;
        }

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
        public float AngleInRadians
        {
            get;
            set;
        }

        [CozParameter(3)]
        public CozPathMotionProfile MotionProfile
        {
            get;
            set;
        }

        [CozParameter(4)]
        public byte Level
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
