using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.driveOffChargerContacts)]
    public class ActionDriveOffChargerContacts : IRobotActionUnion
    {
        public static readonly ActionDriveOffChargerContacts Default = new ActionDriveOffChargerContacts();
    }
}
