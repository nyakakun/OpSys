using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL
{
    public class Reader
    {
        string data = "";
        string lastLex = "";
        string currentLex = "";
        string nextLex = "";
        int currentPosition = 0;
        public string LastLex { get { return lastLex; } }
        public string CurrentLex { get { return currentLex; } }
        public string LowerCurrentLex { get { return currentLex.ToLower(); } }
        public string NextLex { get { return nextLex; } }
        public string LowerNextLex { get { return nextLex.ToLower(); ; } }
        public Reader(StreamReader input)
        {
            SetNewData(input);
        }
        public Reader(string input)
        {
            SetNewData(input);
        }

        public void SetNewData(StreamReader input)
        {
            data = input.ReadToEnd();
            currentPosition = 0;
            lastLex = "";
            currentLex = "";
            nextLex = "";
            Read();
            Read();
        }

        public void SetNewData(string input)
        {
            data = input;
            currentPosition = 0;
            lastLex = "";
            currentLex = "";
            nextLex = "";
            Read();
            Read();
        }

        public bool Read()
        {
            StringBuilder result = new();
            lastLex = currentLex; currentLex = nextLex;
            while (currentPosition < data.Length && (data[currentPosition] == ' ' || data[currentPosition] == '\r' || data[currentPosition] == '\n' || data[currentPosition] == '\t')) currentPosition++;
            if (currentPosition >= data.Length) { nextLex = ""; return false; }
            if (char.IsLetterOrDigit((char)data[currentPosition])) result.Append(ReadWord());
            else result.Append((char)data[currentPosition++]);
            nextLex = result.ToString();
            return (currentPosition >= data.Length) && (nextLex == "");
        }

        private string ReadWord()
        {
            StringBuilder result = new("");
            do result.Append((char)data[currentPosition++]); while (currentPosition < data.Length && char.IsLetterOrDigit((char)data[currentPosition]));
            return result.ToString();
        }
    }
}
