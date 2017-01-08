using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x35, IsEvent = true)]
    public class RobotObservedPossibleObject
    {
        public RobotObservedPossibleObject()
        {
        }

        [CozParameter(0)]
        public RobotObservedObject PossibleObject
        {
            get;
            set;
        }
    }
}
