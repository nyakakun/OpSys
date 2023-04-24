namespace minimizer
{
    internal class Program
    {
        internal class Jump
        {
            public int NextState = 0;
            public int OutputSignal = 0;
        }

        static void Main(string[] args)
        {
            try
            {
                string? line;
                line = Console.ReadLine();
                if (line == null) throw new("А где?");
                string[] KMN = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (!int.TryParse(KMN[0], out int K)) throw new("K - не является числом");
                if (!int.TryParse(KMN[1], out int M)) throw new("M - не является числом");
                if (!int.TryParse(KMN[2], out int N)) throw new("N - не является числом");

                AutomatMinimazed automat = new(K, M);
                AutomatRW RW = new(automat);

                RW.Read(Console.In, M, K);
                RW.VisualizeGraph("satrted.png");

                RW.Write(Console.Out);

                Automat minAuto = automat.Minimize();
                RW.Automat = minAuto;

                RW.Write(Console.Out);
                RW.VisualizeGraph("minimazed.png");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}