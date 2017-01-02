using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Enums
{
    public enum ObjectFamilyTypes : int
    {
        Invalid = -1,
        Unknown = 0,
        Block = 1,
        LightCube = 2,
        Ramp = 3,
        Charger = 4,
        Mat = 5,
        MarkerlessObject = 6,
        CustomObject = 7
    }

    public enum ObjectType : int
    {
        Invalid = -1,
        Unknown = 0,
        Block_LIGHTCUBE1 = 1,
        Block_LIGHTCUBE2 = 2,
        Block_LIGHTCUBE3 = 3,
        FlatMat_GEARS_4x4 = 4,
        FlatMat_LETTERS_4x4 = 5,
        FlatMat_ANKI_LOGO_8BIT = 6,
        FlatMat_LAVA_PLAYTEST = 7,
        Platform_LARGE = 8,
        Bridge_LONG = 9,
        Bridge_SHORT = 10,
        Ramp_Basic = 11,
        Charger_Basic = 12,
        ProxObstacle = 13,
        CliffDetection = 14,
        CollisionObstacle = 15,
        Custom_STAR5_Cube = 16,
        Custom_STAR5_Box = 17,
        Custom_ARROW_Cube = 18,
        Custom_ARROW_Box = 19,
        Custom_Fixed = 20
    }

    public enum CubeObjectType : int
    {
        Block_LIGHTCUBE1 = 1,
        Block_LIGHTCUBE2 = 2,
        Block_LIGHTCUBE3 = 3,
    }
}
