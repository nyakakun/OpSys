using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinator
{
    public class CompaeredArray : IEquatable<CompaeredArray>
    {
        public List<string> Array;

        public CompaeredArray(List<string> array) { this.Array = array; }

        public bool Equals(CompaeredArray? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (this.Count != other.Count) return false;

            bool result = true;

            for (int index = 0; (index < Count) && result; index++) result = this[index] == other[index];

            return result;
        }

        public override int GetHashCode()
        {
            int HASH = 0;
            foreach (var signal in Array) HASH ^= signal.GetHashCode();
            return HASH;
        }

        public string this[int index]
        {
            get { return Array[index]; }
            set { Array[index] = value; }
        }

        public int Count => Array.Count;
    }
}
