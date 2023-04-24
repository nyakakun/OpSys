using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimizer
{
    public class StateMinAuto: State
    {
        protected int startStateNum;
        public StateMinAuto(int stateNum, int jumpCount) : base(stateNum, jumpCount) { startStateNum = stateNum; }
        protected override void AddJump(int nextState, int signal)
        {
            jumps.Add(new JumpMinAuto(nextState, signal));
        }
        public int StartStateNum { get { return startStateNum; } }
        public List<int> Signals
        {
            get
            {
                List<int> result = new List<int>();
                foreach (JumpMinAuto jump in jumps)
                {
                    result.Add(jump.Signal);
                }
                return result;
            }
        }
        public List<int> NextStates
        {
            get
            {
                List<int> result = new List<int>();
                foreach (JumpMinAuto jump in jumps)
                {
                    result.Add(jump.NextState);
                }
                return result;
            }
        }
    }
}
