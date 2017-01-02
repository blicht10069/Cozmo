using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.EventObjects;

namespace CozmoAPI
{
    public class ImageStreamManager
    {
        private CozImage mCurrent = null;
        private object mSync = new object();

        public ImageStreamManager()
        {
        }

        public CozImage AddChunk(ImageChunk chunk)
        {
            CozImage ret = null;
            lock (mSync)
            {
                if (mCurrent == null || mCurrent.ImageID != chunk.ImageID)
                    mCurrent = new CozImage(chunk.ImageID);
                mCurrent.AddChunk(chunk);
                if (mCurrent.IsReady)
                {
                    if (mCurrent.IsGoodImage)
                        ret = mCurrent;
                    else
                        mCurrent = null;
                }
            }
            return ret;
        }
    }

    public class CozImage
    {
        private List<ImageChunk> mChunks;
        private byte[] mImage = null;

        public CozImage(int imageId)
        {
            ImageID = imageId;
            mChunks = new List<ImageChunk>();
            IsReady = false;
            IsGoodImage = false;
        }

        public int ImageID
        {
            get;
            private set;
        }

        public void AddChunk(ImageChunk chunk)
        {
            mChunks.Add(chunk);
            if (chunk.ImageChunkCount > 0)
            {
                IsGoodImage = mChunks.Count == chunk.ImageChunkCount;
                IsReady = true;
            }
        }

        public bool IsGoodImage
        {
            get;
            private set;
        }

        public bool IsReady
        {
            get;
            private set;
        }

        public Bitmap CreateBitmap()
        {
            MemoryStream ms = new MemoryStream(Image);
            Bitmap bmp = (Bitmap)Bitmap.FromStream(ms);
            return new Bitmap(bmp);
        }

        public Bitmap OverlayObservedFaceInformation(RobotObservedFace faceInfo)
        {
            Bitmap ret = CreateBitmap();
            using (Graphics g = Graphics.FromImage(ret))
            using (Font font = new Font("Arial", 16f))
            {
                g.DrawString(faceInfo.Name + " - " + faceInfo.FacialExpression, font, Brushes.Black, PointF.Empty);
                DrawFaceMarker(g, faceInfo.RightEye);
                DrawFaceMarker(g, faceInfo.LeftEye);
                DrawFaceMarker(g, faceInfo.Nose);
                DrawFaceMarker(g, faceInfo.Mouth);
            }
            return ret;
        }
        
        private void DrawFaceMarker(Graphics g, CozPoint[] points)
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
        public byte[] Image
        {
            get
            {
                if (mImage == null)
                {
                    mChunks.Sort();
                    int width = mChunks[0].Resolution.Width;
                    int height = mChunks[0].Resolution.Height;
                    Console.WriteLine("Image {0}x{1}", width, height);
                    MemoryStream ms = new MemoryStream();
                    foreach (ImageChunk ic in mChunks)
                        ms.Write(ic.Data, 0, ic.Data.Length);
                    byte[] rawOldFormat = ms.ToArray();
                    mImage = new byte[HEADER.Length + rawOldFormat.Length*2];
                    for (int i = 0; i < HEADER.Length; i++)
                        mImage[i] = HEADER[i];
                    mImage[0x53] = (byte)(height >> 8);
                    mImage[0x5f] = (byte)(height & 0xFF);
                    mImage[0x60] = (byte)(width >> 8);
                    mImage[0x61] = (byte)(width & 0xFF);

                    int oldLen = rawOldFormat.Length-1;
                    while (oldLen > 0 && rawOldFormat[oldLen - 1] == 0xFF) oldLen--;

                    int offset = HEADER.Length;
                    for (int i = 0; i < oldLen; i++, offset++)
                    {
                        mImage[offset] = rawOldFormat[i+1];
                        if (rawOldFormat[i + 1] == 0xFF)
                        {
                            offset++;
                            mImage[offset] = 0;
                        }
                    }
                    mImage[offset] = 0xFF;
                    offset++;
                    mImage[offset] = 0xD9;

                    byte[] ret = new byte[offset+1];
                    Array.Copy(mImage, ret, ret.Length);
                    mImage = ret;
                }
                return mImage;
            }
        }


        private static byte[] HEADER = {
              0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01,
              0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0x10, 0x0B, 0x0C, 0x0E, 0x0C, 0x0A, 0x10, 
              0x0E, 0x0D, 0x0E, 0x12, 0x11, 0x10, 0x13, 0x18, 0x28, 0x1A, 0x18, 0x16, 0x16, 0x18, 0x31, 0x23,
              0x25, 0x1D, 0x28, 0x3A, 0x33, 0x3D, 0x3C, 0x39, 0x33, 0x38, 0x37, 0x40, 0x48, 0x5C, 0x4E, 0x40,
              0x44, 0x57, 0x45, 0x37, 0x38, 0x50, 0x6D, 0x51, 0x57, 0x5F, 0x62, 0x67, 0x68, 0x67, 0x3E, 0x4D,              
              0x71, 0x79, 0x70, 0x64, 0x78, 0x5C, 0x65, 0x67, 0x63, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x01, 0x28, 
              0x01, 0x90, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0xD2, 0x00, 0x00, 0x01, 0x05, 0x01, 0x01,
              0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04,
              0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x10, 0x00, 0x02, 0x01, 0x03, 0x03, 0x02, 0x04, 0x03,
              0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D, 0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12,
              0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08,
              0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0A, 0x16,
              0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
              0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
              0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
              0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98,
              0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6,
              0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4,
              0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA,
              0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01,
              0x00, 0x00, 0x3F, 0x00
                                       };

    }


}
