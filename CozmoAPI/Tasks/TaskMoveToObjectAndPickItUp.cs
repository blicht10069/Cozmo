using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Tasks
{
    public class TaskMoveToObjectAndPickItUp : TaskBase
    {
        private int mObjectId;

        public TaskMoveToObjectAndPickItUp(int objectId, int id = 0, string name = "Move & Pick")
        {
            Id = id;
            Name = name;
            mObjectId = objectId;
        }

        protected override void DoTask(TaskEventArgs e)
        {
            Console.WriteLine("Moving to object {0}", mObjectId);
            e.Stack.Connection.MoveToObject(mObjectId, 20f).Wait();
            Console.WriteLine("Now in pickup mode");
            e.Stack.Connection.PickupObject(mObjectId, numberOfRetries: 3).Wait();
        }
    }
}
