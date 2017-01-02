using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.flipBlock)]
    public class ActionFlipBlock : IRobotActionUnion
    {               
        public ActionFlipBlock()
        {            
            MotionProfile = new CozPathMotionProfile();
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
    }
}
