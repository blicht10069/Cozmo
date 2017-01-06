using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Tasks
{
    public class TaskMoveToObjectAndPickItUp : TaskBase
    {
        private int mObjectID;

        public TaskMoveToObjectAndPickItUp(int objectId, int id = 0, string name = "Move & Pick")
        {
            Id = id;
            Name = name;
            mObjectID = objectId;
        }

        protected override void DoTask(TaskEventArgs e)
        {
            Console.WriteLine("Moving to object {0}", mObjectID);
            e.Stack.Connection.MoveToObject(mObjectID, 20f).Wait();
            Console.WriteLine("Now in pickup mode");
            e.Stack.Connection.PickupObject(mObjectID, numberOfRetries: 3).Wait();
        }
    }
}
