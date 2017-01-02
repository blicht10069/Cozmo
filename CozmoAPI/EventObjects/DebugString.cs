using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x31, IsEvent=true)]
    public class DebugString : IDebugInformation
    {
        public DebugString()
            : base()
        {
        }

        [CozParameter(0)]
        public string Text
        {
            get;
            set;
        }

        public override string ToString()
        {
            if (Text == null)
                return String.Empty;
            else
                return Text;
        }
    }
}
