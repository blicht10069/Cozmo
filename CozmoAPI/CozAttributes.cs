using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CozFunctionAttribute : Attribute
    {
        public CozFunctionAttribute()
            : base()
        {
            IsUnregisteredFunction = true;
        }

        public CozFunctionAttribute(ActionType actionType)
            : this((byte)actionType)
        {
        }

        public CozFunctionAttribute(byte id)
            : this()
        {
            FunctionId = id;
            IsEvent = false;
            IsUnregisteredFunction = false;
        }

        public byte FunctionId
        {
            get;
            private set;
        }

        public bool IsEvent
        {
            get;
            set;
        }

        public bool IsUnregisteredFunction
        {
            get;
            private set;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CozParameterAttribute : Attribute
    {
        public CozParameterAttribute(int paramIndex)
            : base()
        {
            OverrideType = null;
            Index = paramIndex;
            IsFixedByteSize = false;
            ArrayOfSpecialType = typeof(void);
            LengthPrefixDataType = typeof(byte);
        }

        public int Index
        {
            get;
            private set;
        }

        public Type OverrideType
        {
            get;
            set;
        }

        public bool IsFixedByteSize
        {
            get;
            set;
        }

        public bool IsSpecialTypeArray
        {
            get { return ArrayOfSpecialType != typeof(void); }
        }

        public Type ArrayOfSpecialType
        {
            get;
            set;
        }

        public Type LengthPrefixDataType
        {
            get;
            set;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CozEventAttribute : Attribute
    {
        public CozEventAttribute(byte eventId)
            : base()
        {
            EventId = eventId;
        }

        public byte EventId
        {
            get;
            private set;
        }
    }

    public class CozParameterField : IComparable<CozParameterField>
    {
        private FieldInfo mFieldInfo = null;
        private PropertyInfo mPropertyInfo = null;

        protected CozParameterField(CozParameterAttribute parameter)
            : base()
        {
            Parameter = parameter;
        }

        public CozParameterField(CozParameterAttribute parameter, FieldInfo field)
            : this(parameter)
        {
            mFieldInfo = field;
        }

        public CozParameterField(CozParameterAttribute parameter, PropertyInfo property)
            : this(parameter)
        {
            mPropertyInfo = property;
        }

        public CozParameterAttribute Parameter
        {
            get;
            private set;
        }

        public object GetValue(object source)
        {
            object ret;
            if (mFieldInfo != null)
                ret = mFieldInfo.GetValue(source);
            else
                ret = mPropertyInfo.GetValue(source, null);
            return ret;
        }

        public void SetValue(object source, object val)
        {
            if (mFieldInfo != null)
                mFieldInfo.SetValue(source, val);
            else
                mPropertyInfo.SetValue(source, val, null);
        }


        public Type ParameterType
        {
            get
            {
                Type ret;
                if (Parameter.OverrideType != null)
                    ret = Parameter.OverrideType;
                else if (mFieldInfo != null)
                    ret = mFieldInfo.FieldType;
                else
                    ret = mPropertyInfo.PropertyType;
                return ret;
            }
        }

        public int CompareTo(CozParameterField other)
        {
            return Parameter.Index.CompareTo(other.Parameter.Index);
        }

    }

    public class CozParameterFieldCollection : Collection<CozParameterField>
    {
        public CozParameterFieldCollection()
            : base()
        {
        }

        public CozParameterFieldCollection(Type t)
            : this()
        {
            List<CozParameterField> temp = new List<CozParameterField>();
            foreach (FieldInfo fi in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object[] attributes = fi.GetCustomAttributes(typeof(CozParameterAttribute), false);
                foreach (CozParameterAttribute attribute in attributes)
                    temp.Add(new CozParameterField(attribute, fi));                
            }
            foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object[] attributes = pi.GetCustomAttributes(typeof(CozParameterAttribute), false);
                foreach (CozParameterAttribute attribute in attributes)
                    temp.Add(new CozParameterField(attribute, pi));                
            }
            temp.Sort();
            foreach (CozParameterField pa in temp)
                Add(pa);
        }
    }
}
