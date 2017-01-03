using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Enums
{
    // Written from the perspective of looking from behind with Cozmo facing away
    // Also, it looks like the Left and Right lights can only display Red
    public enum BackpackLightID
    {
        Left = 0,
        Top = 1,
        Middle = 2,
        Bottom = 3,
        Right = 4,
        
    }
}
