using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.World
{
    public class CozObjectCollection : IEnumerable<CozObject>
    {
        private Dictionary<int, CozObject> mLookup;

        public CozObjectCollection()
        {
            mLookup = new Dictionary<int, CozObject>();
        }

        public int Count
        {
            get { return mLookup.Count; }
        }

        public CozObject this[int objectId]
        {
            get
            {
                CozObject ret;
                if (!mLookup.TryGetValue(objectId, out ret)) ret = null;
                return ret;
            }
        }

        public IEnumerator<CozObject> GetEnumerator()
        {
            foreach (CozObject obj in mLookup.Values)
                yield return obj;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (CozObject obj in mLookup.Values)
                yield return obj;
        }
    }

    public class CozObject
    {
        public CozObject(object source)
        {
        }

        public int ObjectId
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            return ObjectId;
        }

        public override bool Equals(object obj)
        {
            CozObject other = obj as CozObject;
            return other != null && other.ObjectId == ObjectId;
        }
    }
}
