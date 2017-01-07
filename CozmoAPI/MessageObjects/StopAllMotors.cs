using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.StopAllMotors)]
    public class StopAllMotors
    {
        public static readonly StopAllMotors Default = new StopAllMotors();
    }
}
