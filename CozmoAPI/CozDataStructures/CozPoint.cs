using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CozmoAPI.CozDataStructures
{
    [CozFunction]
    public class CozPoint
    {
        public CozPoint()
        {
        }

        [CozParameter(0)]
        public float X
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float Y
        {
            get;
            set;
        }
    }

    public class CozPointCollection : Collection<CozPoint>
    {
        public CozPointCollection()
            : base()
        {
        }

        public void Add(float x, float y)
        {
            Add(new CozPoint() { X = x, Y = y });
        }
    }

}
