using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimizer
{
    public abstract class Jump
    {
        protected int nextState;
        protected int signal;

        public Jump(int nextState, int signal)
        {
            this.nextState = nextState;
            this.signal = signal;
        }
        public abstract int NextState { get; set; }
        public abstract int Signal { get; set; }
        public static bool operator ==(Jump left, Jump right) => left.NextState == right.NextState && left.Signal == right.Signal;
        public static bool operator !=(Jump left, Jump right) => !(left.NextState == right.NextState && left.Signal == right.Signal);

    }
}
