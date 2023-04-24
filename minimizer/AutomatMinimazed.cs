using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimizer
{
    public class AutomatMinimazed: Automat
    {
        internal class CompaeredArray : IEquatable<CompaeredArray>
        {
            private List<int> Array;

            public CompaeredArray(List<int> array) { this.Array = array; }

            public bool Equals(CompaeredArray? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (this.Count != other.Count) return false;

                bool result = true;

                for (int index = 0; (index < Count) && result; index++)
                    result = this[index] == other[index];

                return result;
            }

            public override int GetHashCode()
            {
                int HASH = 0;
                foreach(var signal in Array) HASH ^= signal.GetHashCode();
                return HASH;
            }

            public int this[int index]
            {
                get { return Array[index]; }
                set { Array[index] = value; }
            }

            public int Count => Array.Count; 
        }

        private int lastCtateCount;
        public AutomatMinimazed(int stateCount, int jumpCount)
            : base(stateCount, jumpCount)
        { lastCtateCount = 0; }
        public override void AddState(int jumpCount)// => base.AddState(state == null ? new StateMinAuto(Count) : state);
        {
            states.Add(new StateMinAuto(Count, jumpCount));
            //lastCtateCount++;
        }
        public Automat Minimize()
        {
            ChangeStateOnSignals();
            int currentCount = Count;
            do { lastCtateCount = currentCount; currentCount = ChangeStateOnJump(); }
            while (currentCount != lastCtateCount);

            Automat result = new AutomatMinimazed(currentCount, this[0].Count);
            List<CompaeredArray> arrays = new();
            for (int index = 0; index < Count; index++)
            {
                CompaeredArray current = new(((StateMinAuto)this[index]).Signals
                    .Concat(((StateMinAuto)this[index]).NextStates)
                    .ToList());
                int indexNewState = arrays.IndexOf(current);
                if (indexNewState == -1)
                {
                    arrays.Add(current);
                    for (int indexJump = 0; indexJump < this[index].Count; indexJump++)
                    {
                        result[arrays.Count - 1][indexJump].Signal = this[index][indexJump].Signal;
                        result[arrays.Count - 1][indexJump].NextState = this[index][indexJump].NextState;
                    }
                }
            }

            return result;
        }
        private void Normalize()
        {
            for (var indexState = 0; indexState < Count; indexState++)
            {
                for (var indexJump = 0; indexJump < this[indexState].Count; indexJump++)
                {
                    if (this[indexState][indexJump].NextState < 0) continue;
                    this[indexState][indexJump].NextState = 
                        this[((JumpMinAuto)this[indexState][indexJump]).StartNextState].StateNum;
                }
            }
        }
        private void ChangeStateOnSignals()
        {
            List<CompaeredArray> arrays = new();
            for (int index = 0; index < Count; index++)
            {
                CompaeredArray current = new(((StateMinAuto)this[index]).Signals);
                int indexNewState = arrays.IndexOf(current);
                if (indexNewState != -1) this[index].StateNum = indexNewState;
                else { arrays.Add(current); this[index].StateNum = arrays.Count - 1; }
            }
            Normalize();
        }
        private int ChangeStateOnJump()
        {
            List<CompaeredArray> arrays = new();
            for (int index = 0; index < Count; index++)
            {
                CompaeredArray current = new(((StateMinAuto)this[index]).Signals
                    .Concat(((StateMinAuto)this[index]).NextStates)
                    .ToList());
                int indexNewState = arrays.IndexOf(current);
                if (indexNewState != -1) this[index].StateNum = indexNewState;
                else { arrays.Add(current); this[index].StateNum = arrays.Count - 1; }
            }
            Normalize();
            return arrays.Count;
        }
    }
}
