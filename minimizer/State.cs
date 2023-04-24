using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimizer
{
    public abstract class State
    {
        protected int stateNum;
        protected List<Jump> jumps;

        public State(int stateNum, int jumpCount)
        {
            jumps = new List<Jump>();
            this.stateNum = stateNum;
            for (int index = 0; index < jumpCount; index++) AddJump(-1, -1);
        }
        protected abstract void AddJump(int nextState, int signal);
        public Jump this[int index]
        {
            get { return jumps[index]; }
        }
        public int Count => jumps.Count;
        public int StateNum
        {
            get { return stateNum; }
            set { stateNum = value; }
        }
        public static bool operator ==(State left, State right)
        {
            if (left.Count != right.Count) return false;
            for(int index = 0; index < left.Count; index++) if (left[index] != right[index]) return false;
            return true;
        }
        public static bool operator !=(State left, State right)
        {
            if (left.Count != right.Count) return true;
            for (int index = 0; index < left.Count; index++) if (left[index] != right[index]) return true;
            return false;
        }
    }
}
