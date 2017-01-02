using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI
{
    public class RobotEventArgs : EventArgs
    {
        protected RobotEventArgs()
            : base()
        {
        }

        public RobotEventArgs(CozEventType et, object data)
            : this()
        {
            EventType = et;
            Data = data;
        }

        public CozEventType EventType
        {
            get;
            protected set;
        }

        public object Data
        {
            get;
            protected set;
        }
    }


    public class ImageEventArgs : EventArgs
    {
        public ImageEventArgs(CozImage image)
            : base()
        {
            Image = image;
        }

        public CozImage Image
        {
            get;
            private set;
        }
    }
}
