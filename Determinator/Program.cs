using System.Linq;
using System.Text;
using static Determinator.Program.Auto.State;

namespace Determinator
{
    internal class Program
    {
        public class Auto
        {
            Dictionary<string, State> states = new();
            public class State
            {
                public List<Transition> transitions = new();
                public class Transition
                {
                    public List<string> NextState = new();
                    public override string ToString()
                    {
                        StringBuilder result = new();

                        result.Append(string.Join(",", NextState));
                        if (result.Length == 0) result.Append("-");

                        return result.ToString();
                    }
                }

                public void AddTransition(string nextState, int inputSingnal)
                {
                    if (nextState != "-") transitions[inputSingnal].NextState.Add(nextState);
                }
                public void AddTransition() => 
                    transitions.Add(new());
                public override string ToString()
                {
                    StringBuilder result = new();

                    result.Append(string.Join(" ", transitions));

                    return result.ToString();
                }
            }
            public void AddState(string state) => 
                states.Add(state, new());
            public void AddTransition(string state) => 
                states[state].AddTransition();
            public void AddTransition(string state, string nextState, int inputSingnal) => 
                states[state].AddTransition(nextState, inputSingnal);
            public List<string> GetAllNextStates(List<string> state, int inputSignal)
            {
                List<string> nextStateList = new();
                List<string> eNextStateList = new();
                eNextStateList.AddRange(state);
                for (int index = 0; index < eNextStateList.Count; index++)
                {
                    string currentState = eNextStateList[index];
                    foreach (var nextState in states[currentState].transitions[inputSignal].NextState)
                        if (!nextStateList.Contains(nextState)) nextStateList.Add(nextState);

                    foreach (var nextState in states[currentState].transitions[^1].NextState)
                        if (!eNextStateList.Contains(nextState)) eNextStateList.Add(nextState);
                }
                return nextStateList;
            }
            public override string ToString()
            {
                StringBuilder result = new();

                foreach (var state in states) result.AppendLine(state.Value.ToString());

                return result.ToString();
            }
            public DeterminateAuto Determinate()
            {
                DeterminateAuto result = new();
                List<CompaeredArray> newStates = new() { new(new() { states.First().Key }) };

                for (int newState = 0; newState < newStates.Count; newState++)
                {
                    string currentState = $"{newState}";
                    result.AddState(currentState);
                    for (int inputSignal = 0; inputSignal < states.First().Value.transitions.Count - 1; inputSignal++)
                    {
                        result.AddTransition(currentState);
                        CompaeredArray currentNextState = new(GetAllNextStates(newStates[newState].Array, inputSignal));
                        if (currentNextState.Count == 0) continue;
                        currentNextState.Array.Sort();
                        if (!newStates.Contains(currentNextState)) newStates.Add(currentNextState);
                        result.AddTransition(currentState, $"{newStates.IndexOf(currentNextState)}", inputSignal);
                    }
                }

                //for (int indexTrans = 0; indexTrans < states.First().Value.transitions.Count - 1; indexTrans++)
                //    newStates.Add(GetAllNextStates(new() { states.First().Key }, indexTrans));

                return result;
            }
        }

        public class DeterminateAuto
        {
            Dictionary<string, State> states = new();
            public class State
            {
                public List<Transition> transitions = new();
                public class Transition
                {
                    public string NextState = "-";
                    public override string ToString()
                    {
                        return NextState;
                    }
                }

                public void AddTransition(string nextState, int inputSingnal)
                {
                    transitions[inputSingnal].NextState = nextState;
                }
                public void AddTransition() => transitions.Add(new());
                public override string ToString()
                {
                    StringBuilder result = new();

                    result.Append(string.Join(" ", transitions));

                    return result.ToString();
                }
            }
            public void AddState(string state) => states.Add(state, new());
            public void AddTransition(string state) => states[state].AddTransition();
            public void AddTransition(string state, string nextState, int inputSingnal) => states[state].AddTransition(nextState, inputSingnal);
            public override string ToString()
            {
                StringBuilder result = new();

                foreach (var state in states) result.AppendLine(state.Value.ToString());

                return result.ToString();
            }
        }
        static void Main(string[] args)
        {
            try
            {
                string? line = Console.ReadLine();
                while (line == "") { line = Console.ReadLine(); }
                if (line == null) throw new("А где?!?");
                string[] km = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (km.Length != 2) throw new("Ожидалось 2 числа с размерностью автомата");
                if (!int.TryParse(km[0], out int k)) throw new("Ожидалось число");
                if (!int.TryParse(km[1], out int m)) throw new("Ожидалось число");
                Auto auto = new();

                for(int indexState = 0; indexState < k; indexState++)
                {
                    auto.AddState($"{indexState}");
                    line = Console.ReadLine();
                    while (line == "") { line = Console.ReadLine(); }
                    if (line == null) throw new("Недостаточно состояний");
                    string[] transitions = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (transitions.Length == m || transitions.Length == m + 1)
                    {
                        for (int indexInputSignal = 0; indexInputSignal < transitions.Length; indexInputSignal++)
                        {
                            auto.AddTransition($"{indexState}");
                            foreach(string concreteNextState in transitions[indexInputSignal].Split(",", StringSplitOptions.RemoveEmptyEntries))
                            {
                                auto.AddTransition($"{indexState}", concreteNextState, indexInputSignal);
                            }
                        }
                        if (transitions.Length == m) auto.AddTransition($"{indexState}");
                    }
                    else throw new("Неверное количество входных сигналов в состоянии: " + indexState);
                }

                //Console.WriteLine(auto);
                Console.WriteLine(auto.Determinate());
            }
            catch { }
        }
    }
}