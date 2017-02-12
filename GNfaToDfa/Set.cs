namespace GNfaToDfa
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a set of data.
    /// </summary>
    /// <typeparam name="T">The type of data in the set.</typeparam>
    public class Set<T> : ICollection<T>
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        /// <remarks>
        /// The set is represented as the collection of keys of a Dictionary.
        /// Only the keys matter; the type bool used for the value is arbitrary.
        /// </remarks>
        private readonly Dictionary<T, bool> _dictionary;

        public Set()
        {
            _dictionary = new Dictionary<T, bool>();
        }

        public Set(T x)
            : this()
        {
            Add(x);
        }

        public Set(IEnumerable<T> coll)
            : this()
        {
            foreach (T x in coll)
                Add(x);
        }

        public Set(T[] arr)
            : this()
        {
            foreach (T x in arr)
                Add(x);
        }

        public bool Contains(T x)
        {
            return _dictionary.ContainsKey(x);
        }

        public void Add(T x)
        {
            if (!Contains(x))
                _dictionary.Add(x, false);
        }

        public bool Remove(T x)
        {
            return _dictionary.Remove(x);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public void CopyTo(T[] arr, int i)
        {
            _dictionary.Keys.CopyTo(arr, i);
        }

        // Is this set a subset of that?
        public bool Subset(Set<T> that)
        {
            foreach (T x in this)
                if (!that.Contains(x))
                    return false;
            return true;
        }

        // Create new set as intersection of this and that
        public Set<T> Intersection(Set<T> that)
        {
            Set<T> res = new Set<T>();
            foreach (T x in this)
                if (that.Contains(x))
                    res.Add(x);
            return res;
        }

        // Create new set as union of this and that
        public Set<T> Union(Set<T> that)
        {
            Set<T> res = new Set<T>(this);
            foreach (T x in that)
                res.Add(x);
            return res;
        }

        // Compute hash code -- should be cached for efficiency
        public override int GetHashCode()
        {
            int res = 0;
            foreach (T x in this)
                res ^= x.GetHashCode();
            return res;
        }

        public override bool Equals(Object that)
        {
            if (that is Set<T>)
            {
                Set<T> thatSet = (Set<T>)that;
                return thatSet.Count == this.Count
                  && thatSet.Subset(this) && this.Subset(thatSet);
            }
            else
                return false;
        }

        public override String ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append("{ ");
            bool first = true;
            foreach (T x in this)
            {
                if (!first)
                    res.Append(", ");
                res.Append(x);
                first = false;
            }
            res.Append(" }");
            return res.ToString();
        }
    }
}
