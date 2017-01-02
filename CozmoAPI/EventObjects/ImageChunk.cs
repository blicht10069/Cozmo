using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.EventObjects
{
    public enum ImageEncoding : byte
    {
        NoneImageEncoding = 0,
        RawGray = 1,
        RawRGB = 2,
        YUYV = 3,
        BAYER = 4,
        JPEGGray = 5,
        JPEGColor = 6,
        JPEGColorHalfWidth = 7,
        JPEGMinimizedGray = 8,
        JPEGMinimizedColor = 9
    }

    [CozFunction(15, IsEvent=true)]
    public class ImageChunk :IDebugInformation, IComparable<ImageChunk>
    {
        [CozParameter(3)]
        private byte mImageEncoding;

        public ImageChunk()
            : base()
        {
        }

        [CozParameter(0)]
        public int FrameTimeStamp
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int ImageID
        {
            get;
            set;
        }

        [CozParameter(2)]
        public int ChunkDebug
        {
            get;
            set;
        }


        public ImageEncoding ImageEncoding
        {
            get { return (ImageEncoding)mImageEncoding; }
        }

        [CozParameter(4)]
        private byte ImageResolution = 0;        
        public CozImageResolution Resolution
        {
            get { return CozImageResolution.ResolutionModes[ImageResolution]; }            
        }

        [CozParameter(5)]
        public byte ImageChunkCount
        {
            get;
            set;
        }

        [CozParameter(6)]
        public byte ChunkId
        {
            get;
            set;
        }

        [CozParameter(7)]
        public short Status
        {
            get;
            set;
        }

        [CozParameter(8)]
        public byte[] Data
        {
            get;
            set;
        }

        public int CompareTo(ImageChunk other)
        {
            return ChunkId.CompareTo(other.ChunkId);
        }
    }

    public class CozImageResolution
    {
        public static readonly CozImageResolution[] ResolutionModes = new CozImageResolution[]
        {
            new CozImageResolution(16, 16),
            new CozImageResolution(40, 60),
            new CozImageResolution(80, 120),
            new CozImageResolution(160, 240),
            new CozImageResolution(320, 296),
            new CozImageResolution(400, 480),
            new CozImageResolution(640, 480),
            new CozImageResolution(800, 600),
            new CozImageResolution(1024, 768),
            new CozImageResolution(1280, 960),
            new CozImageResolution(1600, 1200),
            new CozImageResolution(2048, 1536),
            new CozImageResolution(3200, 2400)
        };

        public CozImageResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }
    }

}
