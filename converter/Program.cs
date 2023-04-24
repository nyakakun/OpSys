using System.Text;

namespace converter
{
    internal class Program
    {
        class TransitionMili : IEquatable<TransitionMili>, IComparable<TransitionMili>
        {
            public string State = "";
            public string Signal = "";

            public TransitionMili(string state = "", string signal = "")
            {
                State = state;
                Signal = signal;
            }
            public int CompareTo(TransitionMili? other)
            {
                if (other == null) return 1;
                int res = State.CompareTo(other.State);
                if (res == 0) res = Signal.CompareTo(other.Signal);

                return res;
            }
            public bool Equals(TransitionMili? other)
            {
                if (other == null) return false;
                if (other.Signal == Signal && other.State == State) return true;
                return false;
            }
            public override string ToString()
            {
                return String.Format("{0} {1}\n", State, Signal);
            }
            public override bool Equals(object obj) => Equals(obj as TransitionMili);
            public override int GetHashCode() => State.GetHashCode() ^ Signal.GetHashCode();
        }
        class AutoMur
        {
            class State
            {
                List<string> transitions = new();
                public string Signal = "";
                public string this[int inputSignal]
                {
                    get => (inputSignal >= 0 && inputSignal < transitions.Count) ? transitions[inputSignal] : "-";
                }
                public void Add(string nextState) { transitions.Add(nextState); }
                public override string ToString()
                {
                    return String.Format("{0} {1}", Signal, String.Join(" ", transitions));
                }
            }

            Dictionary<string, State> states = new();

            public string GetSignal(string state) => states.ContainsKey(state) ? states[state].Signal : "-";
            public void SetSignal(string state, string signal) { if (states.ContainsKey(state)) states[state].Signal = signal; }
            public void AddTransition(string state, string nextState) { if (states.ContainsKey(state)) states[state].Add(nextState); }
            public void AddState(string state, string signal) { states[state] = new() { Signal = signal }; }
            public override string ToString()
            {
                StringBuilder result = new();
                foreach (string key in states.Keys) result.AppendLine($"{key}\t{states[key].ToString()}");
                return result.ToString();
            }
        }


        class AutoMili
        {
            Dictionary<string, List<TransitionMili>> _dictionary = new();

            public void Push(string state, string nextState, string signal)
            {
                TransitionMili transition = new();
                transition.State = nextState;
                transition.Signal = signal;
                if (!_dictionary.ContainsKey(state)) _dictionary[state] = new List<TransitionMili>();
                _dictionary[state].Add(transition);
            }

            public AutoMur ConvertToMur()
            {
                List<TransitionMili> transitions = new();
                foreach (string key in _dictionary.Keys)
                    foreach (TransitionMili transition in _dictionary[key])
                        transitions.Add(transition);
                transitions.Sort();
                transitions = transitions.Distinct().ToList();

                TransitionMili empty = new("-", "-");
                int indexEmpty = transitions.IndexOf(empty);
                if (indexEmpty != -1) transitions.RemoveAt(indexEmpty);

                AutoMur mur = new();
                for (int index = 0; index < transitions.Count; index++)
                {
                    string currentState = "q" + index;
                    mur.AddState(currentState, transitions[index].Signal);
                    foreach(TransitionMili transition in _dictionary[transitions[index].State])
                    {
                        if (transition.State == "-") mur.AddTransition(currentState, "-");
                        else
                        {
                            int newState = transitions.IndexOf(transition);
                            mur.AddTransition(currentState, "q" + newState);
                        }
                    }
                }
                return mur;
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
                AutoMili auto = new AutoMili();

                for (int indexK = 0; indexK < k; indexK++)
                {
                    string currentSate = "S" + indexK;
                    line = Console.ReadLine();
                    while (line == "") { line = Console.ReadLine(); }
                    if (line == null) throw new("Недостаточно состояний");
                    string[] transitions = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (transitions.Length != m) throw new("Неверное количество входных сигналов в состоянии: " + currentSate);

                    foreach (string transition in transitions)
                    {
                        string[] stateSignal = transition.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        if (stateSignal.Length == 1 && stateSignal[0] == "-") auto.Push(currentSate, stateSignal[0], stateSignal[0]);
                        else if (stateSignal.Length != 2) throw new("Некорректные данные о переходе в состоянии: " + currentSate);
                        else auto.Push(currentSate, stateSignal[0], stateSignal[1]);
                    }
                }
                Console.Write(auto.ConvertToMur().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}