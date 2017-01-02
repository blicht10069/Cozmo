using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;
using CozmoAPI.EventObjects;
using CozmoAPI.MessageObjects;

namespace CozmoAPI
{
    public class CozConnection
    {
        private static int mIdTagGenerator = 10000;

        private string mAdbPath;
        private string mAdbExe;
        private int mCozmoSDKPort;
        private Socket mSocket;
        private ManualResetEvent mReset = new ManualResetEvent(false);
        private Thread mReader;
        private Action<RobotEventArgs> mRobotEvent;
        private Action<ImageEventArgs> mImageEvent;
        private ImageStreamManager mImageStreamManager;

        public CozConnection(string adbLocation = @"E:\cozmo\platform-tools\", int cozmoSDKPort = 5106)
        {
            if (!adbLocation.EndsWith("\\")) adbLocation = adbLocation + "\\";
            mImageStreamManager = new ImageStreamManager();
            Output = TextWriter.Null;
            mAdbPath = adbLocation;
            mAdbExe = mAdbPath + "adb.exe";
            if (!File.Exists(mAdbExe))
                throw new ArgumentException("Could not find adb.exe on the path supplied: " + adbLocation);
            mCozmoSDKPort = cozmoSDKPort;
            mReader = new Thread(RunReader);
            mReader.IsBackground = true;
            mReader.Start();            
        }

        public event Action<RobotEventArgs> RobotEvent
        {
            add { mRobotEvent += value; }
            remove { mRobotEvent -= value; }
        }

        public event Action<ImageEventArgs> ImageEvent
        {
            add { mImageEvent += value; }
            remove { mImageEvent -= value; }
        }

        public TextWriter Output
        {
            get;
            set;
        }

        private string GetDeviceName()
        {
            string ret = null;
            ProcessStartInfo info = new ProcessStartInfo(mAdbExe);
            info.Arguments = "devices";
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            Process p = Process.Start(info);
            using (StreamReader r = new StreamReader(p.StandardOutput.BaseStream))
            {
                r.ReadLine();
                string line = r.ReadLine();
                while (line != null)
                {
                    if (line.Trim().EndsWith("device"))
                    {
                        ret = line.Substring(0, 8);
                        break;
                    }
                }
            }
            return ret;
        }

        public void Disconnect()
        {
            ExecAdb("forward --remove tcp:{0}", mCozmoSDKPort);
            if (mSocket != null)
            {
                mSocket.Disconnect(true);
                mSocket = null;
                mReset.Reset();
            }
        }

        public void Connect()
        {
            string serialNumber = GetDeviceName();
            if (String.IsNullOrEmpty(serialNumber))
                throw new Exception("Could not find a connected device to bridge with adb");
            ExecAdb("-s {1} forward tcp:{0} tcp:{0}", mCozmoSDKPort, serialNumber);
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, mCozmoSDKPort);
            mSocket.Connect(ep);
            mReset.Set();
        }

        protected SingleAction CreateQueueSingleAction()
        {
            QueueSingleAction qsa = new QueueSingleAction()
            {
                IdTag = Interlocked.Increment(ref mIdTagGenerator)
            };
            CozAsyncResult result = qsa.GetAsyncResult(this);
            return new SingleAction(qsa, result);
        }

        public CozAsyncResult Speak(string text, int playEvent = 316, byte voiceStyle = 2, float duration = 1.8f, float pitch = 0f, bool fitToDuration = false)
        {
            return ExecuteAction(
                new ActionSayText()
                {
                    Text = text,
                    DurationScalar = duration,
                    PlayEvent = playEvent,
                    VoicePitch = pitch,
                    VoiceStyle = voiceStyle,
                    FitToDuration = fitToDuration
                }
            );
        }

        public CozAsyncResult Move(float speedMMPS = 10f, float distanceMM = 50f, bool showAnimation = false)
        {
            return ExecuteAction(
                new ActionDriveStraight()
                {
                    SpeedMMPS = speedMMPS,
                    DistMM = distanceMM,
                    ShouldPlayAnimation = showAnimation
                }
            );            
        }

        public CozAsyncResult Turn(double angle, float turnSpeed = 1.57f, float acceleration = 0f, bool isAbsolute = false)
        {
            return ExecuteAction(
                new ActionTurnInPlace()
                {
                    AngleInRad = ActionTurnInPlace.ToRadians(angle),
                    SpeedRadPerSec = turnSpeed,
                    AccelerationRadPerSec2 = acceleration,
                    IsAbsolute = isAbsolute
                });
        }

        public CozAsyncResult MoveToObject(int objectId, float distanceFromObject = 0f, bool useManualSpeed = false, bool usePreDockPose = false, CozPathMotionProfile motionProfile = null)
        {
            ActionGotoObject action = new ActionGotoObject() 
            { 
                ObjectId = objectId, 
                DistanceFromObjectMM = distanceFromObject, 
                UseManualSpeed = useManualSpeed, 
                UsePreDockPose = usePreDockPose 
            };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }

        public CozAsyncResult MoveToPosition(float x, float y, float angleInRadians = 0f, byte level = 0, bool useManualSpeed = false, CozPathMotionProfile motionProfile = null)
        {
            ActionGotoPose action = new ActionGotoPose()
            {
                X = x, Y = y, AngleInRadians = angleInRadians, Level = level, UseManualSpeed = useManualSpeed
            };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }

        public CozAsyncResult SetHeadAngle(float angleInRad, float speed = ActionSetHeadAngle.DEF_SPEED, float acceleration = 0f, float durationInSeconds = 0)
        {
            return ExecuteAction(
                new ActionSetHeadAngle()
                {
                    AngleRad = angleInRad,
                    MaxSpeedRadPerS = speed,
                    AccelRadPerS2 = acceleration,
                    DurationSeconds = durationInSeconds
                });
        }

        public CozAsyncResult SetLiftHeight(float heightMM, float speedInRadPerSec = 0f, float accelInRadPerSec2 = 0f, float durationInSeconds = 0.3f)
        {
            return ExecuteAction(new ActionSetLiftHeight() { HeightMM = heightMM, MaxSpeedRadPerS = speedInRadPerSec, AccelRadPerS2 = accelInRadPerSec2, DurationSeconds = durationInSeconds });
        }

        public CozAsyncResult PickupObject(int objectId, float approachingAngleRad = 0f, bool useApproachingAngle = false,
            bool usePreDockPose = false, bool useManualSpeed = false,
            CozPathMotionProfile motionProfile = null)
        {
            ActionPickupObject action = new ActionPickupObject()
            {
                ObjectId = objectId,
                ApproachAngleRad = approachingAngleRad,
                UseApproachAngle = useApproachingAngle,
                UsePreDockPose = usePreDockPose,
                UseManualSpeed = useManualSpeed
            };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }

        public CozAsyncResult DropObject(float x, float y)
        {
            return ExecuteAction(new ActionPlaceObjectOnGround() { X = x, Y = y });
        }

        public CozAsyncResult DropObjectHere()
        {
            return ExecuteAction(new ActionPlaceObjectOnGroundHere());
        }

        public CozAsyncResult PanAndTilt(float bodyPan = 0f, float headTilt = 0f, bool isPanAbsolute = false, bool isTiltAbsolute = false)
        {
            return ExecuteAction(new ActionPanAndTilt() { BodyPan = bodyPan, HeadTilt = headTilt, IsPanAbsolute = isPanAbsolute, IsTiltAbsolute = isTiltAbsolute });
        }

        public CozAsyncResult AlignWithObject(int objectId)
        {
            return ExecuteAction(new ActionAlignWithObject() { ObjectID = objectId });
        }

        public CozAsyncResult CalibrateMotors(bool headMotor, bool liftMotor)
        {
            return ExecuteAction(new ActionCalibrateMotors() { CalibrateHead = headMotor, CalibrateLift = liftMotor });
        }

        public CozAsyncResult DriveOffChargerContacts()
        {
            return ExecuteAction(ActionDriveOffChargerContacts.Default);
        }

        public CozAsyncResult DisplayText(string text, int durationInMS = 1000, int x = 5, int y = 5, float fontSize = 16f, string fontName = "Ariel")
        {
            ActionDisplayFaceImage img = new ActionDisplayFaceImage();
            img.DurationMS = durationInMS;
            Bitmap bmp = ActionDisplayFaceImage.CreateBitmap();
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawString(text, new Font(fontName, fontSize), Brushes.White, new PointF(x, y));
            }
            img.FromImage(bmp);
            return ExecuteAction(img);
        }

        public CozAsyncResult MountCharger()
        {
            return ExecuteAction(
                new ActionMountCharger()
                {
                    ObjectID = 4,
                });
        }

        public CozAsyncResult EnrollFace(int faceId, string name)
        {
            return ExecuteAction(new ActionEnrollNamedFace() { FaceID = faceId, Name = name });
        }

        public CozAsyncResult FlipBlock(int objectId, CozPathMotionProfile motionProfile = null)
        {
            ActionFlipBlock action = new ActionFlipBlock() { ObjectID = objectId };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }

        public void AbortCommand(IRobotCommand command)
        {
            AbortByIdTag(command.IdTag);
        }

        public void AbortByIdTag(int idTag)
        {
            AbortAction aa = new AbortAction() { IdTag = idTag };
            SendCommand(aa);
        }

        public void RequestImage(ImageSendMode mode = ImageSendMode.SingleShot)
        {
            SendCommand(new ImageRequest() { Mode = mode });
        }

        public CozAsyncResult ExecuteAction(IRobotActionUnion action)
        {
            SingleAction ret = CreateQueueSingleAction();
            ret.QueueSingleAction.Action = action;
            SendCommand(ret.QueueSingleAction);
            return ret.Result;
        }

        public void SendCommand(object messageSource)
        {
            byte[] message = CozFunctionObjectCollection.Default.BuildMessage(messageSource);
            Send(message);
        }

        protected void SendText(string text)
        {            
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            Send(buffer);
        }

        protected void Send(byte[] buffer)
        {
            short size = (short)buffer.Length;
            NetworkStream ns = new NetworkStream(mSocket);
            BinaryWriter w = new BinaryWriter(ns);            
            w.Write(size);
            w.Flush();
            ns.Write(buffer, 0, buffer.Length);
            ns.Flush();
        }

        public void ConnectClean()
        {
            Disconnect();
            Connect();
        }

        private void ExecAdb(string parameters, params object[] data)
        {
            ProcessStartInfo info = new ProcessStartInfo(mAdbExe);
            info.Arguments = String.Format(parameters, data);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(info);
            p.WaitForExit();
        }

        private void RunReader()
        {
            while (true)
            {
                try
                {
                    RunReaderInternal();
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                }
            }
        }
        
        private void RunReaderInternal()
        {
            if (mReset.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Socket socket = mSocket;
                if (socket != null)
                {                    
                    NetworkStream ns = new NetworkStream(socket);
                    BinaryReader reader = new BinaryReader(ns);
                    short size = reader.ReadInt16();
                    byte[] buf = reader.ReadBytes(size);
                    MemoryStream ms = new MemoryStream(buf);
                    object result = CozFunctionObjectCollection.Default.GetObjectFromMessage(ms);
                    CozEventType et = (CozEventType)buf[0];
                    switch (et)
                    {
                        case CozEventType.EngineErrorCodeMessage:
                        case CozEventType.DebugString:
                        case CozEventType.RobotProcessedImage:
                        case CozEventType.RobotState:
                        case CozEventType.Ping:
                        case CozEventType.MoodState:
                        case CozEventType.RobotObservedMotion:
                            break;
                        default:
                            Output.WriteLine((CozEventType)buf[0]);
                            break;
                    }
                    if (result != null)
                    {
                        if (et == CozEventType.ImageChunk)
                        {
                            CozImage img = mImageStreamManager.AddChunk((ImageChunk)result);
                            if (img != null)
                            {
                                if (mImageEvent != null)
                                    mImageEvent(new ImageEventArgs(img));
                            }
                        }
                        if (result is IDebugInformation && !result.ToString().Contains("NoneBehavior"))
                            Output.WriteLine(result);
                        if (mRobotEvent != null)
                            mRobotEvent(new RobotEventArgs((CozEventType)buf[0], result));
                    }
                }
            }
        }
    }

    public class SingleAction
    {
        public SingleAction(QueueSingleAction action, CozAsyncResult result)
        {
            QueueSingleAction = action;
            Result = result;
        }

        public QueueSingleAction QueueSingleAction
        {
            get;
            protected set;
        }

        public CozAsyncResult Result
        {
            get;
            protected set;
        }
    }
}
