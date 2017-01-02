using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.panAndTilt)]
    public class ActionPanAndTilt : IRobotActionUnion
    {
        public ActionPanAndTilt()
            : base()
        {
        }

        [CozParameter(0)]
        public float BodyPan
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float HeadTilt
        {
            get;
            set;
        }

        [CozParameter(2)]
        public bool IsPanAbsolute
        {
            get;
            set;
        }

        [CozParameter(3)]
        public bool IsTiltAbsolute
        {
            get;
            set;
        }
    }
}
