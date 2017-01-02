using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;
using CozmoAPI.EventObjects;

namespace CozmoAPI.MessageObjects
{
    public interface IRobotActionUnion
    {
    }

    public interface IRobotCommand
    {
        int IdTag { get; }
    }

    [CozFunction(98)]
    public class QueueSingleAction : IRobotCommand
    {
        public QueueSingleAction()
        {
            RobotId = 1;
            IdTag = 200000;
            NumberOfRetries = 0;
            Position = 0;
        }

        [CozParameter(0)]
        public byte RobotId
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int IdTag
        {
            get;
            set;
        }

        [CozParameter(2)]
        public byte NumberOfRetries
        {
            get;
            set;
        }

        [CozParameter(3)]
        public byte Position
        {
            get;
            set;
        }

        [CozParameter(4)]
        public IRobotActionUnion Action
        {
            get;
            set;
        }

        public CozAsyncResult GetAsyncResult(CozConnection connection)
        {
            return new CozAsyncResult(connection, this, o =>
                {
                    return o.EventType == CozEventType.RobotCompletedAction && ((RobotCompletedAction)o.Data).IdTag == IdTag;
                }
            );
        }
    }
}
