using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimizer
{
    public class JumpMinAuto : Jump
    {
        protected int startNextState;
        //protected int startSignal;
        public JumpMinAuto(int nextState = -2, int signal = -1) : base(nextState, signal)
        {
            startNextState = nextState;
            //startSignal = signal;
        }
        public int StartNextState
        {
            get { return startNextState; }
            set
            {
                startNextState = value;
                NextState = value;
            }
        }
        public override int Signal
        {
            get { return signal; }
            set
            {
                //if (startSignal == -1) startSignal = value;
                signal = value;
            }
        }
        public override int NextState
        {
            get { return nextState; }
            set
            {
                if (startNextState == -1) startNextState = value;
                nextState = value;
            }
        }
    }
}
