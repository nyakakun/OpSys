using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Xml;

namespace Lexer
{
    public class Lexer
    {
        string data;
        string lastToken;
        string currentToken;
        string nextToken;

        string lastLex;
        string currentLex;
        string nextLex;

        int currentPosition;
        ConfigureLexer lexes;
        public string LastToken { get { return lastToken; } }
        public string CurrentToken { get { return currentToken; } }
        public string NextToken { get { return nextToken; } }
        public string LastLex { get { return lastLex; } }
        public string CurrentLex { get { return currentLex; } }
        public string NextLex { get { return nextLex; } }
        public string LowerCurrentToken { get { return currentToken.ToLower(); } }
        public string LowerNextToken { get { return nextToken.ToLower(); ; } }
        public Lexer(StreamReader input)
        {
            data = input.ReadToEnd();
            lexes = new();
            currentPosition = 0;
            lastToken = "";
            currentToken = "";
            nextToken = "";
            lastLex = "";
            currentLex = "";
            nextLex = "";
            Read();
            Read();
        }

        public bool Read()
        {
            StringBuilder resultToken = new();
            StringBuilder resultLex = new();

            lastToken = currentToken; currentToken = nextToken;
            lastLex = currentLex; currentLex = nextLex;
            while (currentPosition < data.Length && (
                data[currentPosition] == ' ' || 
                data[currentPosition] == '\r' || 
                data[currentPosition] == '\n' || 
                data[currentPosition] == '\t')) 
                currentPosition++;

            if (currentPosition >= data.Length) { nextToken = ""; nextLex = ""; return false; }
            if (char.IsLetter((char)data[currentPosition]) || (char)data[currentPosition] == '_')
            {
                string type = ReadWord();
                resultLex.Append(type);
                type = lexes.GetKeyWordType(type);
                resultToken.Append(type == "" ? "Identifier" : type);
                //Не смог вынести, что-то базовое
            }
            else if (char.IsDigit((char)data[currentPosition]))
            {
                string num = ReadDigit();
                resultLex.Append(num);
                resultToken.Append(num.Contains('.') ? "Float" : "Integer");
                //Не смог вынести, надо юзать регулярку, но примечания в задании о регуляркаx - не понял
            }
            else
            {
                var len = lexes.ContainOperation($"{(char)data[currentPosition]}");
                if (len == 1)
                {
                    resultToken.Append(lexes.GetOperation($"{(char)data[currentPosition]}"));
                    resultLex.Append($"{(char)data[currentPosition]}");
                    currentPosition++;
                }
                else if (len > 1)
                {
                    string type = ReadSymbols();
                    resultLex.Append(type);
                    type = lexes.GetOperation(type);
                    resultToken.Append(type);
                }
                else
                {
                    len = lexes.ContainStrings($"{(char)data[currentPosition]}");
                    string start = "";
                    string end = "";
                    if (len == 1)
                    {
                        end = lexes.GetEndString($"{(char)data[currentPosition]}");
                        resultLex.Append(ReadString(end[0]));
                        resultToken.Append("String");
                    }
                    else if (len > 1)
                    {
                        start = ReadSymbols();
                        end = lexes.GetEndString(start);
                        resultLex.Append(start);
                        string type = ReadString(end[0]);
                        resultLex.Append(ReadString(end[0]));
                        resultToken.Append("String");
                    }
                    else
                    {
                        string error = ReadSymbols();
                        resultLex.Append(error);
                        resultToken.Append("Error");
                    }
                }
            }
            nextLex = resultLex.ToString();
            nextToken = resultToken.ToString();
            return true;
        }
        private string ReadDigit()
        {
            StringBuilder result = new("");
            do result.Append((char)data[currentPosition++]); while (currentPosition < data.Length && (char.IsDigit((char)data[currentPosition]) || (char)data[currentPosition] == '.'));
            return result.ToString();
        }
        private string ReadString(char end)
        {
            StringBuilder result = new(""); 
            result.Append((char)data[currentPosition++]);
            do result.Append((char)data[currentPosition++]); while (currentPosition < data.Length && (char)data[currentPosition - 1] != end);
            return result.ToString();
        }
        private string ReadSymbols()
        {
            StringBuilder result = new("");
            do result.Append((char)data[currentPosition++]); while (currentPosition < data.Length && !char.IsLetterOrDigit((char)data[currentPosition]) && !(data[currentPosition] == ' ' || data[currentPosition] == '\r' || data[currentPosition] == '\n' || data[currentPosition] == '\t'));
            return result.ToString();
        }
        private string ReadWord()
        {
            StringBuilder result = new("");
            do result.Append((char)data[currentPosition++]); while (currentPosition < data.Length && (char.IsLetterOrDigit((char)data[currentPosition]) || (char)data[currentPosition] == '_'));
            return result.ToString();
        }
    }

    internal class ConfigureLexer
    {
        Dictionary<string, string> KeyWords;
        Dictionary<string, string> Strings;
        Dictionary<string, string> Operations;
        public ConfigureLexer()
        {
            KeyWords = new Dictionary<string, string>();
            Strings = new Dictionary<string, string>();
            Operations = new Dictionary<string, string>();
            string? line = null;

            StreamReader input = new("Tokens/KeyWords.txt");
            while ((line = input.ReadLine()) != null)
            {
                line = line.Trim();
                if (line == "") continue;
                string[] keyValuePair = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (keyValuePair.Length != 2) continue;
                KeyWords.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
            }
            input.Close();

            input = new("Tokens/Strings.txt");
            while ((line = input.ReadLine()) != null)
            {
                line = line.Trim();
                if (line == "") continue;
                string[] keyValuePair = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (keyValuePair.Length != 2) continue;
                Strings.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
            }
            input.Close();

            input = new("Tokens/Operations.txt");
            while ((line = input.ReadLine()) != null)
            {
                line = line.Trim();
                if (line == "") continue;
                string[] keyValuePair = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (keyValuePair.Length != 2) continue;
                Operations.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
            }
            input.Close();
        }

        public bool ContainKeyWord(string token) => KeyWords.ContainsKey(token);
        public int ContainStrings(string token)
        {
            int maxLen = 0;
            if (token.Length == 1)
            {
                foreach (var key in Strings.Keys)
                {
                    if (key[0] == token[0]) { maxLen = Math.Max(key.Length, maxLen); }
                }
            }
            else if (Strings.ContainsKey(token)) return token.Length;
            return maxLen;
        }
        public int ContainOperation(string token)
        {
            int maxLen = 0;
            if (token.Length == 1)
            {
                foreach (var key in Operations.Keys)
                {
                    if (key[0] == token[0]) { maxLen = Math.Max(key.Length, maxLen); }
                }
            }
            else if (Operations.ContainsKey(token)) return token.Length;
            return maxLen;
        }
        public string GetEndString(string beginStrings)
        {
            if (Strings.ContainsKey(beginStrings)) return Strings[beginStrings];
            return "";
        }
        public string GetKeyWordType(string beginStrings)
        {
            if (KeyWords.ContainsKey(beginStrings)) return KeyWords[beginStrings];
            return "";
        }
        public string GetOperation(string beginStrings)
        {
            if (Operations.ContainsKey(beginStrings)) return Operations[beginStrings];
            return "";
        }
    }
}