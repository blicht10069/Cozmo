﻿using System;
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
                    RobotObservedObject roo = (RobotObservedObject)e.Data;
                    Console.WriteLine(roo.ObjectID);
                    mBlockID = roo.ObjectID;
                    break;
                case CozEventType.RobotObservedFace:
                    RobotObservedFace rof = (RobotObservedFace)e.Data;
                    mLastObserved = rof;
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
                    mConnection.SetHeadlights(mIsLightOn);
                    break;
            }
        }
       
    }
}
