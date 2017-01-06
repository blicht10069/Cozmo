using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.PlayAnimation)]
    public class ActionPlayAnimation : IRobotActionUnion
    {
        public ActionPlayAnimation()
        {
            RobotID = 1;
            NumberOfLoops = 1;
            AnimationName = String.Empty;
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

        [CozParameter(2)]
        public string AnimationName
        {
            get;
            set;
        }
    }
}
