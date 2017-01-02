using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(100)]
    public class ImageRequest : IRobotActionUnion
    {
        [CozParameter(1)]
        private byte mMode;

        public ImageRequest()
            : base()
        {
            RobotID = 1;
            Mode = ImageSendMode.Off;
        }

        [CozParameter(0)]
        public byte RobotID
        {
            get;
            set;
        }

        
        public ImageSendMode Mode
        {
            get { return (ImageSendMode)mMode; }
            set { mMode = (byte)value; }
        }
    }
}
