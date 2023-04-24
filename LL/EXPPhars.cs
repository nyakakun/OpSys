using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class EXPPhars
    {
        internal class Line
        {
            public string NoTerminal = "";
            public List<string> TriggerSymbols = new();
            public int? Next = new();
            public bool Shift = false;
            public bool Error = false;
            public bool Stackable = false;
            public bool End = false;

            public override string ToString()
            {
                return
                    string.Format("{0}\t{2}\t{3}\t{4}\t{5}\t{6}\t{1}",
                    NoTerminal,
                    string.Join(';', TriggerSymbols),
                    Next == null ? "null" : Next,
                    Shift ? '+' : '-',
                    Error ? '+' : '-',
                    Stackable ? '+' : '-',
                    End ? '+' : '-');
            }
        }
        private Stack<int> _stack = new();
        private int _currentLine = 0;
        private List<Line> _lines = new();
        private Dictionary<int, int> associate = new();
        private Reader _lexer;
        //private bool _end = true;

        public EXPPhars(StreamReader input, StreamReader rules)
        {
            _lexer = new(input);
            if (!ReadLines(rules)) throw new("Ошибка чтения анализатора");
        }
        public EXPPhars(string input, StreamReader rules)
        {
            _lexer = new(input);
            if (!ReadLines(rules)) throw new("Ошибка чтения анализатора");
        }

        public void SetNewData(StreamReader input)
        {
            _lexer.SetNewData(input);
        }

        public void SetNewData(string input)
        {
            _lexer.SetNewData(input);
        }

        public void AddLine(int index,
            string noTerminal,
            List<string> triggerSymbols,
            int? next,
            bool shift,
            bool error,
            bool stackable,
            bool end)
        {
            _lines.Add(new()
            {
                End = end,
                Error = error,
                Next = next,
                NoTerminal = noTerminal,
                Shift = shift,
                Stackable = stackable,
                TriggerSymbols = triggerSymbols
            });
            associate.Add(index, _lines.Count - 1);
        }

        public void Execute(int startIndex)
        {
            _currentLine = startIndex;
            while (_currentLine != 0) ExecuteLine();
        }

        private bool ReadLines(StreamReader input)
        {
            string data = input.ReadToEnd();
            string[] lines = data.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < lines.Length; index++)
            {
                string[] line = lines[index].Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (!int.TryParse(line[0], out int indexLine)) return false;
                string notTerminal = line[1];
                List<string> symbols = new(line[2].Split(','));
                bool shift = line[3] == "+";
                bool error = line[4] == "+";
                int nextInt;
                int? next = null;
                if (line[5] != "null")
                {
                    if (!int.TryParse(line[5], out nextInt))
                        return false;
                    else next = nextInt;
                }
                bool stackable = line[6] == "+";
                bool end = line[7] == "+";
                AddLine(indexLine, notTerminal, symbols, next, shift, error, stackable, end);
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Line line in _lines)
            {
                sb.AppendLine(line.ToString());
            }

            return sb.ToString();
        }

        private bool ExecuteLine()
        {
            string lex = _lexer.CurrentLex;
            if (!associate.ContainsKey(_currentLine))
                throw new($"Ошибка: в анализаторе отсутствует строка с номером: {_currentLine}.");
            Line currentLine = _lines[associate[_currentLine]];
            Console.Write(
                string.Format(" -> {0}\t({1})\t[{2}]\r\n",
                currentLine.NoTerminal, _currentLine, lex));
            if (currentLine.TriggerSymbols.Contains(lex))
            {
                if (currentLine.Shift) _lexer.Read();
                if (currentLine.Stackable) _stack.Push(_currentLine + 1);
                if (currentLine.Next != null) _currentLine = currentLine.Next.Value;
                else if (_stack.Count > 0) _currentLine = _stack.Pop();
                else if (currentLine.End)
                    if (lex == "") _currentLine = 0;
                    else throw new(string.Format("Ошибка: Обнаружены данные после конца разбора"));
                else throw new($"Ошибка: При разборе правила: {currentLine.NoTerminal
                    } по лексеме: [{lex}] нет перехода на следующее правило, при этом стек пуст.");
            }
            else
                if (currentLine.Error) 
                throw new(
                    string.Format("Ошибка: При разборе правила: {0} ожидались следующие лексемы: {1}, но найдена: [{2}].",
                    currentLine.NoTerminal,
                    string.Join(',', currentLine.TriggerSymbols.Select((symbol) => "[" + symbol + "]")),
                    lex));
            else _currentLine++;
            return true;
        }
    }
}