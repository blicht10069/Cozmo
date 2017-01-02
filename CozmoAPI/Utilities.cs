using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI
{
    public class Utilities
    {
        public static float ToRadians(double angle)
        {
            return (float)(Math.PI / 180d * angle);
        }

        public static double ToDegrees(double radians)
        {
            return 180d / Math.PI * (double)radians;
        }

        public static float FeetToMM(double feet)
        {
            return (float)(feet * 12d * 25.4d);
        }

        public static float InchesToMM(double inches)
        {
            return (float)(inches * 25.4d);
        }
    }
}
