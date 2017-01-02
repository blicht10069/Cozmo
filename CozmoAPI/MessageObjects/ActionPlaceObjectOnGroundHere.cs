using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.placeObjectOnGroundHere)]
    public class ActionPlaceObjectOnGroundHere : IRobotActionUnion
    {
    }
}
