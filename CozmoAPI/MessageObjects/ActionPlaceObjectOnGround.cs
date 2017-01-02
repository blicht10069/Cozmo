using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.placeObjectOnGround)]
    public class ActionPlaceObjectOnGround : IRobotActionUnion
    {
        public ActionPlaceObjectOnGround()
            : base()
        {
            MotionProfile = new CozPathMotionProfile();
        }

        [CozParameter(0)]
        public float X
        {
            get;
            set;
        }

        [CozParameter(0)]
        public float Y
        {
            get;
            set;
        }

        [CozParameter(0)]
        public float QW
        {
            get;
            set;
        }

        [CozParameter(0)]
        public float QX
        {
            get;
            set;
        }

        [CozParameter(0)]
        public float QY
        {
            get;
            set;
        }

        [CozParameter(0)]
        public float QZ
        {
            get;
            set;
        }

        [CozParameter(0)]
        public CozPathMotionProfile MotionProfile
        {
            get;
            set;
        }

        [CozParameter(0)]
        public byte Level
        {
            get;
            set;
        }

        [CozParameter(0)]
        public bool UseManualSpeed
        {
            get;
            set;
        }

        [CozParameter(0)]
        public bool UseExactRotation
        {
            get;
            set;
        }

        [CozParameter(0)]
        public bool CheckDestinationFree
        {
            get;
            set;
        }

    }
}
