using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.displayFaceImage)]
    public class ActionDisplayFaceImage : IRobotActionUnion
    {
        public ActionDisplayFaceImage()
        {
            DurationMS = 1000;
            ImageData = new byte[1024];
        }

        public static Bitmap CreateBitmap()
        {
            return new Bitmap(MAX_WIDTH, MAX_HEIGHT);
        }

        [CozParameter(0)]
        public int DurationMS
        {
            get;
            set;
        }

        [CozParameter(1, IsFixedByteSize=true)]
        public byte[] ImageData
        {
            get;
            private set;
        }

        public const int MAX_WIDTH = 128;
        public const int MAX_HEIGHT = 64;

        public bool GetPixel(int col, int row)
        {
            int pixelNumber = row * 128 + col;
            int byteNumber = pixelNumber / 8;
            byte bitNumber = (byte)(Math.Pow(2, 8-(pixelNumber % 8)));
            return (ImageData[byteNumber] & bitNumber) != 0;
        }

        public void SetPixel(int col, int row, bool isOn)
        {
            int pixelNumber = row * 128 + col;
            int byteNumber = pixelNumber / 8;
            byte bitNumber = (byte)(Math.Pow(2, 8- (pixelNumber % 8)));
            if (isOn)
                ImageData[byteNumber] = (byte)(ImageData[byteNumber] | bitNumber);
            else
            {
                bitNumber = (byte)(255 - bitNumber);
                ImageData[byteNumber] = (byte)(ImageData[byteNumber] & bitNumber);
            }
        }

        public void FromImage(Bitmap img)
        {
            for(int x = 0;x<MAX_WIDTH;x++)
                for (int y = 0; y < MAX_HEIGHT; y++)
                {
                    Color c = img.GetPixel(x, y);
                    SetPixel(x, y, c.R != 0 | c.G != 0 | c.B != 0);
                }
        }
    }
}
