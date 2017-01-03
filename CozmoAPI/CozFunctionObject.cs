using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CozmoAPI.Enums;
using CozmoAPI.MessageObjects;

namespace CozmoAPI
{
    public class CozFunctionObjectCollection : Collection<CozFunctionObject>
    {
        private Dictionary<Type, CozFunctionObject> mMessageLookup;
        private Dictionary<CozEventType, CozFunctionObject> mEventLookup;

        public CozFunctionObjectCollection() : base()
        {
            mMessageLookup = new Dictionary<Type, CozFunctionObject>();
            mEventLookup = new Dictionary<CozEventType, CozFunctionObject>();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in types)
            {
                if (t.GetCustomAttributes(typeof(CozFunctionAttribute), false).Length > 0)
                    Add(new CozFunctionObject(t));
            }
        }

        public static readonly CozFunctionObjectCollection Default = new CozFunctionObjectCollection();

        protected override void InsertItem(int index, CozFunctionObject item)
        {
            base.InsertItem(index, item);
            if (item.IsEvent)
                mEventLookup[(CozEventType)item.FunctionId] = item;
            mMessageLookup[item.Type] = item;
        }

        public CozFunctionObject this[Type t]
        {
            get
            {
                CozFunctionObject ret;
                if (!mMessageLookup.TryGetValue(t, out ret)) ret = null;
                return ret;
            }
        }

        public CozFunctionObject this[CozEventType et]
        {
            get
            {
                CozFunctionObject ret;
                if (!mEventLookup.TryGetValue(et, out ret)) ret = null;
                return ret;
            }
        }

        public object GetObjectFromMessage(Stream s)
        {
            object ret = null;
            CozEventType et = (CozEventType)s.ReadByte();
            CozFunctionObject cfo = this[et];            
            if (cfo != null)
            {
                ret = cfo.ReadFromStream(s);
            }
            return ret;
        }

        public byte[] BuildMessage(object source)
        {
            MemoryStream ret = new MemoryStream();
            CozFunctionObject fo = this[source.GetType()];
            if (fo == null)
            {
                fo = new CozFunctionObject(source.GetType());
                Add(fo);
            }
            fo.WriteToStream(ret, source);
            return ret.ToArray();
        }

        public void DebugWriteMessage(TextWriter output, object source)
        {
            DebugWriteMessage(output, (byte[])BuildMessage(source));
        }

        public void DebugWriteMessage(TextWriter output, byte[] data)
        {
            int count = 0;
            foreach (byte ch in data)
            {
                Console.WriteLine("#{2}: {0} {1}", ch.ToString("X2"), (char)ch, count);
                count++;
            }
        }

    }

    public class CozFunctionObject
    {        
        public CozFunctionObject(Type type)
        {
            Type = type;
            object[] attributes = Type.GetCustomAttributes(typeof(CozFunctionAttribute), false);
            if (attributes.Length == 0)
                throw new ArgumentException(String.Format("Type {0} does not have a CozFunction attribute defined, which is required", Type.FullName));
            CozFunctionAttribute cfa = (CozFunctionAttribute)attributes[0];
            FunctionId = cfa.FunctionId;
            IsEvent = cfa.IsEvent;
            IsUnregisteredFunction = cfa.IsUnregisteredFunction;
            Parameters = new CozParameterFieldCollection(Type);
        }

        public bool IsUnregisteredFunction
        {
            get;
            private set;
        }

        public byte FunctionId
        {
            get;
            private set;
        }

        public bool IsEvent
        {
            get;
            private set;
        }

        public Type Type
        {
            get;
            private set;
        }

        public CozParameterFieldCollection Parameters
        {
            get;
            private set;
        }

        public object ReadFromStream(Stream stream)
        {
            object ret = Activator.CreateInstance(Type);
            BinaryReader r = new BinaryReader(stream);
            foreach (CozParameterField param in Parameters)
            {
                if (ret != null)
                {
                    Type pt = param.ParameterType;
                    object val = null;
                    if (param.Parameter.IsSpecialTypeArray)
                    {
                        int size;
                        if (param.Parameter.LengthPrefixDataType == typeof(byte))
                            size = r.ReadByte();
                        else if (param.Parameter.LengthPrefixDataType == typeof(short))
                            size = r.ReadInt16();
                        else
                            size = r.ReadInt32();
                        Array array = Array.CreateInstance(param.Parameter.ArrayOfSpecialType, size);
                        Type arrayElementType = param.Parameter.OverrideType == null ? param.Parameter.ArrayOfSpecialType : param.Parameter.OverrideType;
                        for (int i = 0; i < size; i++)
                        {
                            CozFunctionObject fo = CozFunctionObjectCollection.Default[arrayElementType];
                            if (fo != null)
                                array.SetValue(fo.ReadFromStream(stream), i);
                        }
                        val = array;
                    }
                    else if (pt == typeof(byte))
                        val = r.ReadByte();
                    else if (pt == typeof(short))
                        val = r.ReadInt16();
                    else if (pt == typeof(int))
                        val = r.ReadInt32();
                    else if (pt == typeof(long))
                        val = r.ReadInt64();
                    else if (pt == typeof(float))
                        val = r.ReadSingle();
                    else if (pt == typeof(double))
                    {
                        val = r.ReadSingle();
                        val = Convert.ChangeType(val, typeof(double));
                    }
                    else if (pt == typeof(bool))
                        val = r.ReadByte() == 1;
                    else if (pt == typeof(string))
                    {
                        byte len = r.ReadByte();
                        byte[] sbuf = r.ReadBytes(len);
                        val = Encoding.ASCII.GetString(sbuf);
                    }
                    else if (pt == typeof(byte[]))
                    {
                        short len = r.ReadInt16();
                        byte[] bbuf = r.ReadBytes((short)len);
                        val = bbuf;
                    }
                    else if (pt.IsClass)
                    {
                        CozFunctionObject fo = CozFunctionObjectCollection.Default[pt];
                        if (fo != null)
                        {
                            val = fo.ReadFromStream(stream);                            
                        }
                    }                    
                    param.SetValue(ret, val);
                }
            }
            return ret;
        }

        public void WriteToStream(Stream stream, object source)
        {
            BinaryWriter w = new BinaryWriter(stream);
            if (!IsUnregisteredFunction)
                w.Write(FunctionId);            
            foreach (CozParameterField param in Parameters)
            {
                Type pt = param.ParameterType;
                object val = param.GetValue(source);
                if (param.Parameter.IsFixedByteSize && pt == typeof(byte[]))
                {
                    byte[] fixedArray = (byte[])val;
                    w.Write(fixedArray, 0, fixedArray.Length); 
                }
                else if (pt == typeof(byte))
                    w.Write((byte)val);
                else if (pt == typeof(short))
                    w.Write((short)val);
                else if (pt == typeof(int))
                    w.Write((int)val);
                else if (pt == typeof(long))
                    w.Write((long)val);
                else if (pt == typeof(float))
                    w.Write((float)val);
                else if (pt == typeof(double))
                {
                    float f = (float)Convert.ChangeType(val, typeof(float));
                    w.Write(f);
                }
                else if (pt == typeof(bool))
                    w.Write((bool)val ? (byte)1 : (byte)0);
                else if (pt == typeof(string))
                {
                    string sval = (string)val;
                    if (sval.Length > 255) throw new Exception("Invalid string length: " + val);
                    w.Write((byte)sval.Length);
                    w.Flush();
                    byte[] buf = Encoding.ASCII.GetBytes(sval);
                    stream.Write(buf, 0, buf.Length);
                    stream.Flush();
                }
                else if (pt != typeof(byte[]) && pt.IsArray)
                {
                    Array array = (Array)val;
                    int length = array.GetLength(0);
                    for (int i = 0; i < length; i++)
                    {
                        object arrayElement = array.GetValue(i);
                        byte[] buf = CozFunctionObjectCollection.Default.BuildMessage(arrayElement);
                        stream.Write(buf, 0, buf.Length);
                    }
                }
                else
                {
                    byte[] buf = CozFunctionObjectCollection.Default.BuildMessage(val);
                    stream.Write(buf, 0, buf.Length);
                }

            }
        }
    }
}
