using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CozmoAPI;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;
using CozmoAPI.EventObjects;
using CozmoAPI.MessageObjects;
using CozmoAPI.Tasks;

namespace CozmoAPIExamples
{
    public class VMMain : ViewModelBase
    {
        private CozConnection mConnection;
        private int mBlockID;
        private DispatcherTimer mTimer;
        private CozImage mLastImage = null;
        private RobotObservedFace mLastObserved = null;
        private bool mIsCameraOn = false;
        private bool mIsLightOn = false;
        private ActivityMode mActivityMode = ActivityMode.None;
        private DateTime mNextTiltAndPan = DateTime.MinValue;
        private bool mWorkingOnIt = false;
        private TaskQueue mTaskQueue;
        private bool mIsPatrolling = false;
        private bool mObserveBlock = false;
        private bool mIsNoding = false;
        private CleanRoomManager mCleanRoomManager;

        public VMMain()
            : base()
        {
            // mConnection = new CozConnection(@"c:\andriod_adb_path\adb.exe"); 
            mConnection = new CozConnection(); // pass your path and filename to adb.exe here
            mTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.05), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher);
            Status = "Ready";
            mConnection.Output = TextWriter.Null;
            mConnection.RobotEvent += mConnection_RobotEvent;
            mConnection.ImageEvent += mConnection_ImageEvent;            
        }

        private void Draw(Graphics g, CozPoint[] points)
        {
            if (points.Length == 0) return;
            Point[] pt = new Point[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                pt[i].X = (int)points[i].X;
                pt[i].Y = (int)points[i].Y;
            }
            g.DrawPolygon(Pens.Red, pt);
        }

        private void OnTick(object sender, EventArgs e)
        {
            CozImage ci = mLastImage;
            if (ci != null && mIsCameraOn)
            {               
                MemoryStream ms2 = new MemoryStream();
                if (mLastObserved != null)
                    ci.OverlayObservedFaceInformation(mLastObserved).Save(ms2, ImageFormat.Bmp);
                else
                    ci.CreateBitmap().Save(ms2, ImageFormat.Bmp);
                ms2.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = null;
                bi.StreamSource = ms2;
                bi.EndInit();
                bi.Freeze();                
                Image = bi;
            }
        }

        private BitmapImage mImage = null;
        public BitmapImage Image
        {
            get { return mImage; }
            set
            {
                mImage = value;
                Notify("Image");
            }
        }

        private int mImageCounter = 0;
        private void mConnection_ImageEvent(ImageEventArgs obj)
        {
            mLastImage = obj.Image;
        }

        private void mConnection_RobotEvent(RobotEventArgs e)
        {
            switch (e.EventType)
            {
                case CozEventType.RobotState:
                    break;
                case CozEventType.RobotObservedObject:      
                    if (mObserveBlock)
                    {
                        mObserveBlock = false;
                        RobotObservedObject roo = (RobotObservedObject)e.Data;
                        Console.WriteLine("Found Block {0} at ({1:N2}, {2:N2}, {3:N2}) at {4:N3} degrees",
                            roo.ObjectID, roo.Pose.X, roo.Pose.Y, roo.Pose.Z, roo.Pose.AngleZ);
                        mTaskQueue.Push(e2 =>
                            {
                                CozPoint pt = roo.Pose.CalculateOffsetPosition(-100f);
                                e2.Stack.Connection.MoveToPosition(pt.X, pt.Y, (float)roo.Pose.AngleZRad).Wait();
                            });
                    }
                    /*
                    if (mWorkingOnIt || !mIsPatrolling) return;
                    RobotObservedObject roo = (RobotObservedObject)e.Data;
                    switch (roo.ObjectType)
                    {
                        case ObjectType.Block_LIGHTCUBE1:
                        case ObjectType.Block_LIGHTCUBE2:
                        case ObjectType.Block_LIGHTCUBE3:
                            mWorkingOnIt = true;                            
                            mTaskQueue.Push(new TaskMoveToObjectAndPickItUp(roo.ObjectID));
                            mTaskQueue.AbortCurrentTask();                           
                            break;
                    }
                     */ 
                    break;
                case CozEventType.RobotObservedFace:
                    RobotObservedFace rof = (RobotObservedFace)e.Data;
                    mLastObserved = rof;
                    break;                
                case CozEventType.RobotConnectionResponse:
                    
                    break;
            }
        }


        private string mStatus = String.Empty;
        public string Status
        {
            get { return mStatus; }
            set
            {
                mStatus = value;  
                Notify("Status");
            }
        }

        protected override void DoCommand(object parameter)
        {
            switch (parameter as string)
            {
                case "Connect":
                    mConnection.ConnectClean();
                    mTaskQueue = new TaskQueue(mConnection);        
                    Status = "Connected";
                    break;
                case "Disconnect":
                    mConnection.Disconnect();
                    Status = "Disconnected";
                    break;
                case "SayHello":
                    mTaskQueue.Push(e =>
                        {
                            string[] parts = "Hello Ben, how are you today".Split(' ');
                            float pitch = 0f;
                            foreach (string part in parts)
                            {
                                e.Stack.Connection.Speak(part, pitch: pitch).Wait();
                                pitch += 0.1f;
                            }
                        }
                    );
                    break;
                case "MoveForward":
                    mTaskQueue.Push(e =>
                        {
                            CozAsyncResult wait = e.Stack.Connection.Move(10f, 100f);
                            Thread.Sleep(5000);
                            e.Stack.Connection.AbortCommand(wait.Command); // example of aborting a command early
                            wait.Wait();
                        }
                    );
                    break;
                case "TurnRight":
                    mConnection.Turn(Utilities.ToRadians(90)).Wait();
                    break;
                case "Box":
                    for (int i = 0; i < 4; i++)
                    {
                        mConnection.Move(100, 200).Wait();
                        mConnection.Turn(-90).Wait();
                    }
                    break;
                case "Box2":
                    mConnection.Move(100, 200).Completed=
                        o1 => mConnection.Turn(90).Completed =
                            o2 => mConnection.Move(100, 200).Completed = 
                                o3=> mConnection.Turn(90).Completed = 
                                    o4=> mConnection.Move(100,200).Completed = 
                                        o5=> mConnection.Turn(90).Completed = 
                                            o6 => mConnection.Move(100,200).Completed =
                                                o7=>mConnection.Turn(90).Completed =
                                                    o8 => mConnection.Move(100,200);
                    Status = "Box Queued Up";
                    break;
                case "MountCharger":
                    // This doesn't appear to work, or I've got a bug in my implementation
                    mConnection.MountCharger();
                    break;
                case "ImageRequest":
                    if (mIsCameraOn)
                        mConnection.RequestImage(ImageSendMode.Off);
                    else
                        mConnection.RequestImage(ImageSendMode.Stream);
                    mIsCameraOn = !mIsCameraOn;
                    Status = "Getting Images";
                    break;
                case "Nod":
                    // this shows how easy it is to abort an item on the queue
                    // this code will abort the current task running first.
                    // so click nod, and then in the middle click nod again
                    // notice you do not need to check at every point in the code
                    // if you are aborted.  What happens is the e.Stack.Connection
                    // is replaced when you are aborted.  Your current command is stopped
                    // in the robot, and you are given a null connection
                    // the null connection continues to take the commands,
                    // but does not send them to the robot.  It returns a result
                    // that is immediately complete.
                    if (mIsNoding)
                        mTaskQueue.AbortCurrentTask();
                    else
                    {
                        mIsNoding = true;
                        mTaskQueue.Push(e =>
                            {
                                e.Stack.Connection.SetHeadAngle(Utilities.ToRadians(20), durationInSeconds: 0.2f).Wait();
                                Thread.Sleep(500);
                                for (int i = 0; i < 5; i++)
                                {
                                    e.Stack.Connection.SetHeadAngle(Utilities.ToRadians(30), durationInSeconds: 0.1f).Wait();
                                    e.Stack.Connection.SetHeadAngle(Utilities.ToRadians(10), durationInSeconds: 0.1f).Wait();
                                }
                                Thread.Sleep(500);
                                e.Stack.Connection.SetHeadAngle(Utilities.ToRadians(-20), durationInSeconds: 0.5f).Wait();
                                mIsNoding = false;
                            }
                        );
                    }
                    break;
                case "Calibrate":
                    CozAsyncResult ar = mConnection.CalibrateMotors(true, true);
                    ar.Wait();
                    Status = String.Format("Motors Calibrated (Code: {0})", ar.ResultCode);
                    break;
                case "DisplayALine":                   
                    for (int i = 0; i < 20; i++)
                        mConnection.DisplayTextCentered(String.Format("{0:HH:mm:ss}", DateTime.Now)).Wait();
                    break;
                case "FlipCube2":
                    mConnection.Speak("Preparing").Wait();
                    mConnection.SetHeadAngle(Utilities.ToRadians(5)).Wait();
                    mConnection.Speak("I see the block").Wait();
                    mConnection.MoveToObject(mBlockID).Wait();
                    mConnection.Speak("The block will be flipped").Wait();
                    mConnection.FlipBlock(mBlockID).Wait();
                    Status = "Flipped";
                    break;
                case "MoveToASpecificLocation":
                    mConnection.MoveToPosition(Utilities.FeetToMM(1), Utilities.FeetToMM(1), Utilities.ToRadians(180)).Wait();
                    mConnection.MoveToPosition(Utilities.FeetToMM(0), Utilities.FeetToMM(0), Utilities.ToRadians(0)).Wait();
                    break;                
                case "ToggleLights":
                    mIsLightOn = !mIsLightOn;
                    mConnection.SetBackpackLightState(BackpackLightID.Top, mIsLightOn ? Color.Blue : Color.Black);
                    mConnection.SetBackpackLightState(BackpackLightID.Middle, mIsLightOn ? Color.White : Color.Black);
                    mConnection.SetBackpackLightState(BackpackLightID.Bottom, mIsLightOn ? Color.Green : Color.Black);
                    mConnection.SetBackpackLightState(BackpackLightID.Left, mIsLightOn ? Color.Red : Color.Black);
                    mConnection.SetBackpackLightState(BackpackLightID.Right, mIsLightOn ? Color.Purple : Color.Black);
                    /*
                    SetBackpackLED led = new SetBackpackLED();
                    led.SetLightState(BackpackLightID.Top, mIsLightOn ? Color.Blue : Color.Black);
                    led.SetLightState(BackpackLightID.Middle, mIsLightOn ? Color.White: Color.Black);
                    led.SetLightState(BackpackLightID.Bottom, mIsLightOn ? Color.Green : Color.Black);
                    led.SetLightState(BackpackLightID.Left, mIsLightOn ? Color.Red : Color.Black);
                    led.SetLightState(BackpackLightID.Right, mIsLightOn ? Color.Purple : Color.Black);
                    mConnection.ExecuteCommand(led);
                     */                    
                    break;
                case "NightVision":
                    mIsLightOn = !mIsLightOn;
                    mConnection.SetHeadlights(mIsLightOn);                    
                    break;                
                case "Animate":
                    mConnection.PlayAnimationTrigger(CozAnimationTriggerType.CozmoSaysGetOut).Wait();
                    break;
                case "CleanRoom":
                    if (mCleanRoomManager == null)
                    {
                        CleanRoomSetup setup = new CleanRoomSetup(mConnection);
                        mCleanRoomManager = new CleanRoomManager(setup);
                    }
                    else
                    {
                        mCleanRoomManager.Stop();
                        mCleanRoomManager = null;
                    }
                    break;
                case "Click":
                    PrecisionParking();
                    break;
                case "Patrol":
                    // this toggles patrol on or off
                    // patrol is another example of using the task queue.
                    // the TaskPatrol will take an array of point
                    // and will have Cozmo patrol that polygon
                    // over and over. 
                    // at the same time the method subscribes to the
                    // Robot Event service (calling OnPatrolEvent)
                    // If the Robot spies a Cube, then it stops 
                    // what its doing and goes right for the
                    // cube and picks it up.
                    mIsPatrolling = !mIsPatrolling;
                    if (mIsPatrolling)
                    {
                        mConnection.DriveOffChargerContacts().Wait();
                        mConnection.StopAllMotors();
                        mConnection.RobotEvent += OnPatrolEvent;
                        TaskPatrol task = new TaskPatrol();
                        task.Points = new CozPointCollection()
                        {
                            { Utilities.FeetToMM(-1), 0 },
                            { 0, Utilities.FeetToMM(1.5) },
                            { Utilities.FeetToMM(1), 0 }
                        };
                        mTaskQueue.Push(task);
                    }
                    else
                    {
                        mConnection.RobotEvent -= OnPatrolEvent;
                        mTaskQueue.AbortCurrentTask();
                    }
                    break;
            }

            
        }

        private void OnPatrolEvent(RobotEventArgs e)
        {
            if (e.EventType == CozEventType.RobotObservedObject)
            {
                RobotObservedObject roo = (RobotObservedObject)e.Data;
                switch (roo.ObjectType)
                {
                    case ObjectType.Block_LIGHTCUBE1:
                    case ObjectType.Block_LIGHTCUBE2:
                    case ObjectType.Block_LIGHTCUBE3:
                        mConnection.RobotEvent -= OnPatrolEvent;
                        mTaskQueue.Push(e2 =>
                            {
                                e2.Stack.Connection.Speak("ahoy, I spotted me object!").Wait();
                            }
                        );
                        mTaskQueue.Push(new TaskMoveToObjectAndPickItUp(roo.ObjectID));
                        mTaskQueue.AbortCurrentTask();
                        break;
                }
            }
        }

        // This example demonstrates the new properties and methods added to the CozPose3D object
        // you can now get the Angle in Radians or Degrees of the observed object relatevie to the
        // position of the robot.
        // Also introduced is the CalculateOffsetPosition method which given a distance
        // will calculate the X & Y coordinates of the robot.
        // using these two methods we can get the robot to line up with any cube
        // it spots -- this function will always part the Robot perpedicular 
        // to the Cube's face 100 MM in front.
        // if you change the -100f to 100f your robot will move in front of the cube
        // by 100 MM and park directly in front of it.
        private void PrecisionParking()
        {
            Action<RobotEventArgs> removal = null;
            Action<RobotEventArgs> spy = (e) =>
            {
                if (e.EventType == CozEventType.RobotObservedObject)
                {
                    mConnection.RobotEvent -= removal;
                    RobotObservedObject roo = (RobotObservedObject)e.Data;
                    Console.WriteLine("Found Block {0} at ({1:N2}, {2:N2}, {3:N2}) at {4:N3} degrees",
                        roo.ObjectID, roo.Pose.X, roo.Pose.Y, roo.Pose.Z, roo.Pose.AngleZ);
                    mTaskQueue.Push(e2 =>
                    {
                        CozPoint pt = roo.Pose.CalculateOffsetPosition(-100f);
                        e2.Stack.Connection.MoveToPosition(pt.X, pt.Y, (float)roo.Pose.AngleZRad).Wait();
                    });
                };
            };
            removal = spy;
            mConnection.RobotEvent += spy;
        }

       
    }

    public enum ActivityMode
    {
        None = 0,
        GotoAndPickupObject 
    }
}
