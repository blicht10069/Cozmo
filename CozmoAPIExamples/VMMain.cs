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
        private TaskStack mTaskStack;
        private bool mIsPatrolling = false;

        public VMMain()
            : base()
        {
            mConnection = new CozConnection();
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
                case CozEventType.RobotObservedObject:
                    if (mWorkingOnIt || !mIsPatrolling) return;
                    RobotObservedObject roo = (RobotObservedObject)e.Data;
                    switch (roo.ObjectType)
                    {
                        case ObjectType.Block_LIGHTCUBE1:
                        case ObjectType.Block_LIGHTCUBE2:
                        case ObjectType.Block_LIGHTCUBE3:
                            mWorkingOnIt = true;
                            /*
                            MessageStack.Default.Push(o =>
                                {
                                    Console.WriteLine("Moving in on object: {0}", roo.ObjectID);
                                    mConnection.MoveToObject(roo.ObjectID, 70f).Wait();
                                    Console.WriteLine("Now Picking Up Object {0}", roo.ObjectID);
                                    mConnection.CalibrateMotors(false, true).Wait();
                                    CozAsyncResult action = mConnection.PickupObject(roo.ObjectID, numberOfRetries: 3);
                                    Console.WriteLine("trying to pick up object");
                                    action.Wait();
                                    Console.WriteLine("pick up is {0}", action.ResultCode);
                                    mWorkingOnIt = false;
                                });
                             */
                            mTaskStack.Push(new TaskMoveToObjectAndPickItUp(roo.ObjectID));
                            mTaskStack.AbortCurrentTask();                           
                            break;
                    }
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
                    mTaskStack = new TaskStack(mConnection);        
                    Status = "Connected";
                    break;
                case "Disconnect":
                    mConnection.Disconnect();
                    Status = "Disconnected";
                    break;
                case "SayHello":
                    string[] parts = "Hello Ben, how are you today".Split(' ');
                    float pitch = 0f;
                    foreach (string part in parts)
                    {
                        mConnection.Speak(part, pitch: pitch).Wait();
                        pitch += 0.1f;
                    }
                    break;
                case "MoveForward":
                    CozAsyncResult wait = mConnection.Move(10f, 100f);
                    Status = "Moving...";
                    Thread.Sleep(5000);
                    Status = "Aborting...";
                    mConnection.AbortCommand(wait.Command);
                    wait.Wait();
                    Status = "Done";
                    break;
                case "TurnRight":
                    mConnection.Turn(90).Wait();
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
                    mConnection.SetHeadAngle(Utilities.ToRadians(20), durationInSeconds: 0.2f).Wait();
                    Thread.Sleep(500);
                    for (int i = 0; i < 5; i++)
                    {
                        mConnection.SetHeadAngle(Utilities.ToRadians(30), durationInSeconds: 0.1f).Wait();
                        mConnection.SetHeadAngle(Utilities.ToRadians(10), durationInSeconds: 0.1f).Wait();
                    }
                    Thread.Sleep(500);
                    mConnection.SetHeadAngle(Utilities.ToRadians(-20), durationInSeconds: 0.5f).Wait();
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
                case "PickUpBlock":
                    mConnection.Output = Console.Out;
                    /*
                    int blockId = mBlockID;
                    if (mBlockID > 0)
                    {
                        mConnection.CalibrateMotors(true, true).Wait();
                        mConnection.SetHeadAngle(Utilities.ToRadians(10)).Wait();
                        mConnection.MoveToObject(blockId, 0f).Wait();
                        mConnection.PickupObject(blockId).Wait();
                    }
                    else
                        Status = "No Block Found";
                     */
                    mConnection.SetLiftHeight(150).Wait();
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
                    /*
                    mIsLightOn = !mIsLightOn;
                    mConnection.SetHeadlights(mIsLightOn);
                     */
                    break;
                case "NightVision":
                    mIsLightOn = !mIsLightOn;
                    mConnection.SetHeadlights(mIsLightOn);
                    break;
                case "GotoPickup":
                    if (mActivityMode != ActivityMode.None)
                    {
                        mActivityMode = ActivityMode.None;
                        mConnection.Speak("No longer seeking objects").Wait();
                    }
                    else
                    {
                        mConnection.SetHeadAngle(Utilities.ToRadians(10)).Wait();
                        mConnection.Speak("Yes SIR!").Wait();
                        mActivityMode = ActivityMode.GotoAndPickupObject;
                    }
                    break;
                case "Patrol":
                    mIsPatrolling = !mIsPatrolling;
                    if (mIsPatrolling)
                    {
                        TaskPatrol task = new TaskPatrol();
                        task.Points = new CozPointCollection()
                        {
                            { Utilities.FeetToMM(-1), 0 },
                            { 0, Utilities.FeetToMM(1.5) },
                            { Utilities.FeetToMM(1), 0 }
                        };
                        mTaskStack.Push(task);
                    }
                    else
                        mTaskStack.AbortCurrentTask();
                    break;
            }
        }
       
    }

    public enum ActivityMode
    {
        None = 0,
        GotoAndPickupObject 
    }
}
