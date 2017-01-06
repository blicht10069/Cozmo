using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static string DebugObject(object o)
        {
            StringWriter ret = new StringWriter();
            ret.Write("Type {0}:" + o.GetType().Name);
            foreach (PropertyInfo pi in o.GetType().GetProperties())
            {
                bool isComplexType = (!pi.PropertyType.IsValueType && pi.PropertyType != typeof(string));
                if (isComplexType) ret.Write(" (");
                ret.Write("{0}={1};", pi.Name, pi.GetValue(o, null));
                if (isComplexType) ret.Write(") ");
            }
            return ret.ToString();
        }
    }
}
