using System.Diagnostics.Metrics;

namespace parse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StreamReader reader = new("input.txt");
            Reader input = new Reader(reader);
            try
            {
                Prog(input);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message.ToString());
                return;
            }
            Console.WriteLine("Ошибок не обнаружено");
        }

        public static void Prog(Reader input)
        {
            if (input.LowerCurrentLex != "prog")
                throw new Exception($"Ожидалось: [PROG] а надено: [{input.CurrentLex}]");
            input.Read();
                        
            if (input.LowerCurrentLex != "id")
                throw new($"Ожидалось: [id] а надено: [{input.CurrentLex}]");
            input.Read();

            Var(input);

            if (input.LowerCurrentLex != "begin")
                throw new Exception($"Ожидалось: [BEGIN] а надено: [{input.CurrentLex}]");
            input.Read();

            ListST(input);
            if (input.LowerCurrentLex != "end")
                throw new Exception($"Ожидалось: [END] а надено: [{input.CurrentLex}]");
        }

        public static void Var(Reader input)
        {
            if (input.LowerCurrentLex != "var")
                throw new Exception($"Ожидалось: [VAR] а надено: [{input.CurrentLex}]");
            input.Read();

            IDList(input);

            if (input.LowerCurrentLex != ":")
                throw new Exception($"Ожидалось: [:] а надено: [{input.CurrentLex}]");
            input.Read();

            Type(input);
            if (input.CurrentLex != ";")
                throw new Exception($"Ожидалось: [;] а надено: [{input.CurrentLex}]");
            input.Read();
        }

        public static void IDList(Reader input)
        {
            if (input.LowerCurrentLex != "id")
                throw new Exception($"Ожидалось: [id] а надено: [{input.CurrentLex}]");
            input.Read();
            
            _IDList(input);
        }

        public static void _IDList(Reader input)
        {
            if (input.LowerCurrentLex == ",")
            {
                input.Read();
                IDList(input);
            }
        }

        public static void Type(Reader input)
        {
            switch (input.LowerCurrentLex) 
            {
                case "int":
                case "bool":
                case "string":
                case "float":
                    input.Read();
                    break;
                default: throw new Exception($"Ожидалось: {{[int],[bool],[string],[float]}} а надено: [{input.CurrentLex}]");
            }
        }
        
        public static void ListST(Reader input)
        {
            ST(input);
            _ListSt(input);
        }

        public static void _ListSt(Reader input)
        {
            if(input.LowerCurrentLex != "end")
            {
                ST(input);
                _ListSt(input);
            }
        }

        public static void ST(Reader input)
        {
            switch (input.LowerCurrentLex) 
            {
                case "read":
                    input.Read();
                    Read(input);
                    break;
                case "write":
                    input.Read();
                    Write(input);
                    break;
                case "id":
                    input.Read();
                    Assign(input);
                    break;

                default: throw new Exception($"Ожидалось: {{[read],[write],[id]}} а надено: [{input.CurrentLex}]");
            }
        }

        public static void Assign(Reader input)
        {
            
            if (input.LowerCurrentLex != ":" && input.LowerNextLex != "=")
                throw new Exception($"Ожидалось: [:=] а надено: [{input.CurrentLex}{input.NextLex}]");
            input.Read();
            input.Read();
            Exp(input);
            if (input.LowerCurrentLex != ";")
                throw new Exception($"Ожидалось: [;] а надено: [{input.CurrentLex}]");
            input.Read();
        }

        public static void Exp(Reader input)
        {
            T(input);
            _Exp(input);
        }

        public static void _Exp(Reader input)
        {
            switch (input.LowerCurrentLex) 
            {
                case "+":
                    input.Read();
                    T(input);
                    _Exp(input);
                    break;
                default:
                    break;
            }
        }

        public static void T(Reader input)
        {
            F(input);
            _T(input);
        }

        public static void _T(Reader input)
        {
            switch (input.LowerCurrentLex) 
            {
                case "*":
                    input.Read();
                    F(input);
                    _T(input);
                    break;
                default:
                    break;
            }
        }

        public static void F(Reader input)
        {
            switch (input.LowerCurrentLex) 
            {
                case "-":
                    input.Read();
                    F(input);
                    break;
                case "(":
                    input.Read();
                    Exp(input);
                    if (input.LowerCurrentLex != ")")
                        throw new Exception($"Ожидалось: [)] а надено: [{input.CurrentLex}]");
                    input.Read();
                    break;
                case "id":
                case "num":
                    input.Read();
                    break;
                default : throw new Exception($"Ожидалось: {{[-],[(],[id],[num]}} а надено: [{input.LowerCurrentLex}]");
            }
        }

        public static void Read(Reader input)
        {
            if (input.LowerCurrentLex != "(")
                throw new Exception($"Ожидалось: [(] а надено: [{input.LowerCurrentLex}]");
            input.Read();
            IDList(input);

            if (input.LowerCurrentLex != ")")
                throw new Exception($"Ожидалось: [)] а надено: [{input.LowerCurrentLex}]");
            input.Read();

            if(input.LowerCurrentLex != ";")
                throw new Exception($"Ожидалось: [;] а надено: [{input.LowerCurrentLex}]");
            input.Read();
        }

        public static void Write(Reader input)
        {
            if (input.LowerCurrentLex != "(")
                throw new Exception($"Ожидалось: [(] а надено: [{input.CurrentLex}]");
            input.Read();
            IDList(input);

            if (input.LowerCurrentLex != ")")
                throw new Exception($"Ожидалось: [)] а надено: [{input.CurrentLex}]");
            input.Read();

            if (input.LowerCurrentLex != ";")
                throw new Exception($"Ожидалось: [;] а надено: [{input.CurrentLex}]");
            input.Read();
        }
    }
}