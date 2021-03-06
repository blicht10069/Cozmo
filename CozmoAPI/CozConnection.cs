﻿using System;
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
        private SetBackpackLED mBackpackLedState = new SetBackpackLED();
        private bool mIsNullConnection = false;
        private CozConnection mUnderlyingConnection = null;
        private SingleAction mLastAction = null;
        private CozConnection mUltimateSource = null;
        private CozPathMotionProfile mDefaultMotionProfile = null;
        private HashSet<CozEventWait> mWaits = new HashSet<CozEventWait>();

        protected CozConnection()
        {
        }

        public CozConnection(string adbLocation = @"E:\cozmo\platform-tools\", int cozmoSDKPort = 5106)
            : this()
        {
            if (!adbLocation.EndsWith("\\")) adbLocation = adbLocation + "\\";            
            mImageStreamManager = new ImageStreamManager();
            Output = TextWriter.Null;
            mAdbPath = adbLocation;
            mAdbExe = mAdbPath + "adb.exe";
            if (!File.Exists(mAdbExe))
            {
                string searchPath = TryToFindAdb();
                if (searchPath == null)
                    throw new ArgumentException("Could not find adb.exe on the path supplied: " + adbLocation);
                else
                {
                    mAdbPath = searchPath;
                    mAdbExe = mAdbPath + "adb.exe";
                    Console.WriteLine("Found connection here {0}", mAdbExe);
                }                
            }
            mCozmoSDKPort = cozmoSDKPort;
            mReader = new Thread(RunReader);
            mReader.IsBackground = true;
            mReader.Start();            
        }

        private string TryToFindAdb()
        {
            string ret = null;
            string[] paths = Environment.GetEnvironmentVariable("path").Split(';');
            foreach (string path in paths)
            {
                string temp = path;
                if (!temp.EndsWith("\\")) temp += "\\";
                if (File.Exists(temp + "adb.exe"))
                {
                    ret = temp;
                    break;
                }
            }
            return ret;
        }
        protected Socket Socket
        {
            get
            {
                if (mUltimateSource == null)
                    return mSocket;
                else
                    return mUltimateSource.mSocket;
            }
            set
            {
                if (mUltimateSource == null)
                    mSocket = value;
            }
        }

        public CozConnection Clone()
        {
            CozConnection ret = new CozConnection();
            ret.mAdbExe = mAdbExe;
            ret.mAdbPath = mAdbPath;
            ret.mCozmoSDKPort = mCozmoSDKPort;
            ret.mReset = new ManualResetEvent(false);
            ret.mReader = mReader;
            ret.mUltimateSource = mUltimateSource == null ? this : mUltimateSource;
            ret.mImageStreamManager = mImageStreamManager;
            ret.mBackpackLedState = mBackpackLedState;
            return ret;
        }

        public CozConnection CreateNullConnection()
        {
            CozConnection ret = new CozConnection();
            mUnderlyingConnection = this;
            ret.mIsNullConnection = true;
            return ret;
        }

        public event Action<RobotEventArgs> RobotEvent
        {
            add
            {
                if (mUltimateSource == null)
                    mRobotEvent += value;
                else
                    mUltimateSource.RobotEvent += value;
            }
            remove
            {
                if (mUltimateSource == null)
                    mRobotEvent -= value;
                else
                    mUltimateSource.RobotEvent -= value;
            }
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
                    line = r.ReadLine();
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

        public CozPathMotionProfile DefaultMotionProfile
        {
            get
            {
                if (mDefaultMotionProfile == null)
                    mDefaultMotionProfile = new CozPathMotionProfile();
                return mDefaultMotionProfile;
            }
            set
            {
                mDefaultMotionProfile = value;
            }
        }

        public void SetHeadlights(bool isOn)
        {
            ExecuteCommand(new HeadlightControl() { IsLightOn = isOn });
        }

        public void SetBackpackLightState(BackpackLightID light, Color color)
        {
            mBackpackLedState.SetLightState(light, color);
            ExecuteCommand(mBackpackLedState);
        }

        
        public CozAsyncResult Speak(string text, CozAnimationTriggerType playEvent = CozAnimationTriggerType.Count, CozVoiceStyle voiceStyle = CozVoiceStyle.CozmoProcessing_Sentence, float duration = 1.8f, float pitch = 0f, bool fitToDuration = false)
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

        public CozAsyncResult MoveArc(float speedMMPS = 10f, float radiusMM = 10f)
        {
            return ExecuteAction(new ActionDriveArc() { SpeedMMPS = speedMMPS, RadiusMM = radiusMM });
        }

        public void MoveWheels(float leftWheelMMPS, float rightWheelMMPS, float leftWheelAccelMMPS2 = 0f, float rightWheelMMPS2 = 0f)
        {
            ExecuteCommand(new DriveWheels()
            {
                LeftWheelMMPS = leftWheelAccelMMPS2,
                RightWheelMMPS = rightWheelMMPS,
                LeftWheelAccelMMPS2 = leftWheelAccelMMPS2,
                RightWheelAccelMMPS2 = rightWheelMMPS
            });
        }

        public CozAsyncResult Turn(float angleInRad, float turnSpeed = 1.57f, float acceleration = 0f, bool isAbsolute = false)
        {
            return ExecuteAction(
                new ActionTurnInPlace()
                {
                    AngleInRad = angleInRad,
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
                UsePreDockPose = usePreDockPose,
                MotionProfile = DefaultMotionProfile
            };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }

        public CozAsyncResult MoveToPosition(float x, float y, float angleInRadians = 0f, byte level = 0, bool useManualSpeed = false, CozPathMotionProfile motionProfile = null)
        {
            ActionGotoPose action = new ActionGotoPose()
            {
                X = x, Y = y, AngleInRadians = angleInRadians, Level = level, UseManualSpeed = useManualSpeed, MotionProfile = DefaultMotionProfile
            };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }

        public CozAsyncResult MoveToPositionNoAngle(float x, float y, byte level = 0, bool useManualSpeed = false, CozPathMotionProfile motionProfile = null)
        {
            float angleInRadians = (float)Math.Atan(y / x);
            return MoveToPosition(x, y, angleInRadians, level, useManualSpeed, motionProfile);
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
            bool usePreDockPose = true, bool useManualSpeed = false,
            CozPathMotionProfile motionProfile = null, int numberOfRetries = 0)
        {
            ActionPickupObject action = new ActionPickupObject()
            {
                ObjectID = objectId,
                ApproachAngleRad = approachingAngleRad,
                UseApproachAngle = useApproachingAngle,
                UsePreDockPose = usePreDockPose,
                UseManualSpeed = useManualSpeed,
                MotionProfile = DefaultMotionProfile
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

        public CozAsyncResult PlaceObjectOnObject(int objectId)
        {
            return ExecuteAction(new ActionPlaceOnObject() { ObjectID = objectId, MotionProfile = DefaultMotionProfile });
        }

        public CozAsyncResult PlaceRelativeToObject(int relativeObjectID, float offsetXMM, float approachingAngleRad = 0, bool useApproachingAngle = false)
        {
            return ExecuteAction(new ActionPlaceRelObject()
            {
                ObjectID = relativeObjectID,
                PlacementOffsetXMM = offsetXMM,
                ApproachingAngleRadians = approachingAngleRad,
                UseApproachingAngle = useApproachingAngle,
                MotionProfile = DefaultMotionProfile
            });
        }

        public CozAsyncResult FlipBlock(int objectId, CozPathMotionProfile motionProfile = null)
        {
            ActionFlipBlock action = new ActionFlipBlock() { ObjectID = objectId, MotionProfile = DefaultMotionProfile };
            if (motionProfile != null) action.MotionProfile = motionProfile;
            return ExecuteAction(action);
        }


        public CozAsyncResult AlignWithObject(int objectId)
        {
            return ExecuteAction(new ActionAlignWithObject() { ObjectID = objectId });
        }

        public CozAsyncResult PanAndTilt(float bodyPan = 0f, float headTilt = 0f, bool isPanAbsolute = false, bool isTiltAbsolute = false)
        {
            return ExecuteAction(new ActionPanAndTilt() { BodyPan = bodyPan, HeadTilt = headTilt, IsPanAbsolute = isPanAbsolute, IsTiltAbsolute = isTiltAbsolute });
        }

        public CozAsyncResult CalibrateMotors(bool headMotor, bool liftMotor)
        {
            return ExecuteAction(new ActionCalibrateMotors() { CalibrateHead = headMotor, CalibrateLift = liftMotor });
        }

        public CozAsyncResult DriveOffChargerContacts()
        {
            return ExecuteAction(ActionDriveOffChargerContacts.Default);
        }

        public void StopAllMotors()
        {
            ExecuteCommand(CozmoAPI.MessageObjects.StopAllMotors.Default);
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

        public CozAsyncResult DisplayTextCentered(string text, int durationInMS = 1000, float fontSize = 16f, string fontName = "Lucida Console")
        {
            ActionDisplayFaceImage img = new ActionDisplayFaceImage();
            img.DurationMS = durationInMS;
            Bitmap bmp = ActionDisplayFaceImage.CreateBitmap();
            using (Graphics g = Graphics.FromImage(bmp))
            using (StringFormat fmt = new StringFormat())
            {
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;                                                
                g.DrawString(text, new Font(fontName, fontSize), Brushes.White, new Rectangle(0,0,128,64), fmt);
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

        public CozAsyncResult PlayAnimation(string animationName, int numberOfLoops = 1)
        {
            return ExecuteAction(new ActionPlayAnimation() { NumberOfLoops = numberOfLoops, AnimationName = animationName });
        }

        public CozAsyncResult PlayAnimationTrigger(CozAnimationTriggerType triggerType, int numberOfLoops = 1)
        {
            return ExecuteAction(new ActionPlayAnimationTrigger() { TriggerType = triggerType, NumberOfLoops = numberOfLoops });
        }

        public void AbortCommand(IRobotCommand command)
        {
            AbortByIdTag(command.IdTag);
        }

        public void AbortByIdTag(int idTag)
        {
            AbortAction aa = new AbortAction() { IdTag = idTag };
            ExecuteCommand(aa);
        }

        public void RequestImage(ImageSendMode mode = ImageSendMode.SingleShot)
        {
            ExecuteCommand(new ImageRequest() { Mode = mode });
        }

        public CozEventWait CreateWait(params CozEventType[] events)
        {
            return CreateWait(new CozEventWaitSetup(events));
        }        

        public CozEventWait CreateWait(CozEventWaitSetup setup)
        {
            CozEventWait ret;
            if (mIsNullConnection)
            {
                ret = CozEventWait.CreateNullWait(this, setup);
            }
            else
            {
                ret = new CozEventWait(this, setup);
                lock (mWaits)
                {
                    mWaits.Add(ret);
                }
            }
            return ret;
        }

        public void RemoveWait(CozEventWait wait)
        {
            lock (mWaits)
            {
                mWaits.Remove(wait);
            }
        }

        public void AbortAllWaits()
        {
            CozEventWait[] waits = mWaits.ToArray();
            foreach (CozEventWait wait in waits)
                if (!wait.IsComplete) wait.Abort();
        }

        public CozAsyncResult ExecuteAction(IRobotActionUnion action, byte numberOfRetries = 0)
        {
            SingleAction ret = CreateQueueSingleAction();
            ret.QueueSingleAction.NumberOfRetries = numberOfRetries;
            ret.QueueSingleAction.Action = action;
            if (mIsNullConnection)
            {
                return CozAsyncResult.CreateEmptyResult(this);
            };
            ExecuteCommand(ret.QueueSingleAction);
            mLastAction = ret;
            return ret.Result;
        }

        public SingleAction LastAction
        {
            get { return mLastAction; }
        }

        public void ExecuteCommand(object messageSource)
        {
            if (mIsNullConnection) return;
            byte[] message = CozFunctionObjectCollection.Default.BuildMessage(messageSource);
            Send(message);
        }

        protected void SendText(string text)
        {
            if (mIsNullConnection) return;
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            Send(buffer);
        }

        protected void Send(byte[] buffer)
        {
            if (mIsNullConnection) return;
            short size = (short)buffer.Length;
            NetworkStream ns = new NetworkStream(Socket);
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

        private RobotState mLastRobotState = RobotState.Empty;
        public RobotState GetRobotState()
        {
            if (mUltimateSource == null)
                return mLastRobotState;
            else
                return mUltimateSource.mLastRobotState;
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
                Socket socket = Socket;
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
                        case CozEventType.Ping:
                        case CozEventType.MoodState:
                        case CozEventType.RobotObservedMotion:
                            break;
                        case CozEventType.RobotState:
                            mLastRobotState = (RobotState)result;
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
