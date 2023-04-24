using System.Text;

namespace converterMiliToMur
{
    public class Program
    {
        class AutoMur
        {
            class State
            {
                List<string> transitions = new();
                public string Signal = "";
                public int Count { get => transitions.Count; }
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
            public string this[string state, int inputSignal]
            {
                get
                {
                    return states[state][inputSignal];
                }
            }
            public string GetSignal(string state) => states.ContainsKey(state) ? states[state].Signal : "-";
            public void SetSignal(string state, string signal) { if (states.ContainsKey(state)) states[state].Signal = signal; }
            public void AddTransition(string state, string nextState) { if (states.ContainsKey(state)) states[state].Add(nextState); }
            public void AddState(string state, string signal) { states[state] = new() { Signal = signal }; }
            public override string ToString()
            {
                StringBuilder result = new();
                foreach (string key in states.Keys) result.AppendLine(states[key].ToString());
                return result.ToString();
            }
            public AutoMili ConvertToMili()
            {
                AutoMili autoMili = new AutoMili();
                foreach (string key in states.Keys)
                {
                    string newState = "S" + key[1..];
                    for (int index = 0; index < states[key].Count; index++)
                    {
                        if (states[key][index] == "-") autoMili.Push(newState, "-", "-");
                        else autoMili.Push(newState, "S" + states[key][index][1..], states[states[key][index]].Signal);
                    }
                }
                return autoMili;
            }
        }
        class AutoMili
        {
            Dictionary<string, List<TransitionMili>> _dictionary = new();
            public class TransitionMili
            {
                public string State = "";
                public string Signal = "";

                public TransitionMili(string state = "", string signal = "")
                {
                    State = state;
                    Signal = signal;
                }
                public override string ToString()
                {
                    return String.Format("{0} {1}\n", State, Signal);
                }
            }
            public void Push(string state, string nextState, string signal)
            {
                TransitionMili transition = new();
                transition.State = nextState;
                transition.Signal = signal;
                if (!_dictionary.ContainsKey(state)) _dictionary[state] = new List<TransitionMili>();
                _dictionary[state].Add(transition);
            }
            public string ToString()
            {
                StringBuilder result = new StringBuilder();
                foreach (string key in _dictionary.Keys)
                {
                    foreach(TransitionMili transition in _dictionary[key])
                    {
                        result.Append(
                            string.Format(
                                "{0,2}/{1,2} ",
                                transition.State,
                                transition.Signal
                                )
                            );
                    }
                    result.AppendLine();
                }
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
                AutoMur auto = new();

                for (int indexK = 0; indexK < k; indexK++)
                {
                    string currentSate = "q" + indexK;
                    line = Console.ReadLine();
                    while (line == "") { line = Console.ReadLine(); }
                    if (line == null) throw new("Недостаточно состояний");
                    string[] transitions = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (transitions.Length != m + 1) throw new("Неверное количество входных сигналов в состоянии: " + currentSate);
                    auto.AddState(currentSate, transitions[0]);

                    for (int index = 1; index <= m; index++) auto.AddTransition(currentSate, transitions[index]);
                }
                Console.Write(auto.ConvertToMili().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}