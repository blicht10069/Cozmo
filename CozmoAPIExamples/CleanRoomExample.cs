using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CozmoAPI;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;
using CozmoAPI.EventObjects;
using CozmoAPI.Tasks;

namespace CozmoAPIExamples
{
    public class CleanRoomSetup
    {
        private static Random mRandom = new Random();

        public const int TASK_MONITOR_ROOM = 10;
        public const int TASK_REMOVING_OBJECT = 20;
        private PointF mMiddleOfCloset = PointF.Empty;

        public CleanRoomSetup(CozConnection connection)
        {
            Connection = connection.Clone();
            Room = new RectangleF(0, 0, Utilities.FeetToMM(5f), Utilities.FeetToMM(5f));
            Closet = new RectangleF(0, Utilities.FeetToMM(-0.5f), Utilities.FeetToMM(2f), Utilities.FeetToMM(-1.5f));
        }

        public CozConnection Connection
        {
            get;
            private set;
        }

        public RectangleF Room
        {
            get;
            set;
        }

        public RectangleF Closet
        {
            get;
            set;
        }

        public PointF MiddleOfCloset
        {
            get
            {
                if (mMiddleOfCloset == PointF.Empty)
                {
                    mMiddleOfCloset.X = (Closet.Width + Closet.X) / 2f;
                    mMiddleOfCloset.Y = (Closet.Height + Closet.Y) / 2f;
                }
                return mMiddleOfCloset;
            }
        }

        public ITask CreateMonitorRoomTask()
        {
            CozPathMotionProfile motionProfile = new CozPathMotionProfile();
            motionProfile.SpeedMMPS = 30;            
            return new Task(TASK_MONITOR_ROOM, "Monitor Room", e =>
                {
                    e.Stack.Connection.SetHeadAngle(Utilities.ToRadians(25));
                    while (!e.Stack.IsAborted)
                    {
                        float x = Room.Width * (float)mRandom.NextDouble() + Room.X;
                        float y = Room.Height * (float)mRandom.NextDouble() + Room.Y;
                        Console.WriteLine("Moving to {0}, {1}", x, y);
                        e.Stack.Connection.MoveToPosition(x, y, motionProfile: motionProfile).Wait();
                        Console.WriteLine("Completed Move...now turning");
                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine("{0:HH:mm:ss} Turn Right 36 degrees.. ", DateTime.Now);
                            e.Stack.Connection.Turn(Utilities.ToRadians(36), Utilities.ToRadians(36)).Wait();
                        }
                    }
                }
            );
        }

        public ITask CreateCleanObjecTask()
        {
            return new Task(TASK_REMOVING_OBJECT, "Moving object into closet", e =>
                {
                    e.Stack.Connection.MoveToPosition(MiddleOfCloset.X, MiddleOfCloset.Y, Utilities.ToRadians(180)).Wait();
                    e.Stack.Connection.SetLiftHeight(0).Wait();
                    e.Stack.Connection.Move(-100f, 100).Wait();
                }
            );
        }
    }
    
    public class CleanRoomManager
    {
        private CleanRoomSetup mSetup;
        private TaskQueue mTaskQueue;

        public CleanRoomManager(CleanRoomSetup setup)
        {
            mSetup = setup;
            mTaskQueue = new TaskQueue(mSetup.Connection);
            mTaskQueue.Push(mSetup.CreateMonitorRoomTask());
            mSetup.Connection.RobotEvent += Connection_RobotEvent;
        }

        private void Connection_RobotEvent(RobotEventArgs e)
        {
            if (e.EventType == CozmoAPI.Enums.CozEventType.RobotObservedObject)
            {
                RobotObservedObject roo = (RobotObservedObject)e.Data;
                Console.WriteLine("Spotted object of family type {0}, {1}", roo.FamilyType, roo.ObjectID);
                if (mTaskQueue.CurrentTask.Id == CleanRoomSetup.TASK_MONITOR_ROOM && roo.FamilyType == ObjectFamilyTypes.LightCube)
                {
                    Console.WriteLine("Found Cube {0}, at {1}, {2}, is in closet? {3}",
                        roo.ObjectID, roo.Pose.X, roo.Pose.Y, roo.Pose.X < 0f || roo.Pose.Y < 0f);
                    if (!(roo.Pose.X < 0f || roo.Pose.Y < 0f))
                    {
                        mTaskQueue.Push(new TaskMoveToObjectAndPickItUp(roo.ObjectID));
                        mTaskQueue.Push(mSetup.CreateCleanObjecTask());
                        mTaskQueue.Push(mSetup.CreateMonitorRoomTask());
                        mTaskQueue.AbortCurrentTask();
                    }
                }
            }
        }

        public void Stop()
        {
            mSetup.Connection.RobotEvent -= Connection_RobotEvent;                
            mTaskQueue.AbortAllTasks();
        }
    }
}
