using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimizer
{
    public abstract class Automat
    {
        protected List<State> states;

        public Automat(int stateCount, int jumpCount)
        {
            states = new List<State>();
            for (int index = 0; index < stateCount; index++)
            {
                AddState(jumpCount);
            }
        }
        public State this[int index]
        {
            get { return states[index]; }
        }
        public abstract void AddState(int jumpCount);
        public int Count => states.Count;
    }
}
