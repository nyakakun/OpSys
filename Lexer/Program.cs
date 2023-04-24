namespace Lexer
{
    public class Program
    {
        static void Main(string[] args)
        {
            StreamReader input = new("input.txt");
            Lexer lexer = new(input);
            do
            {
                Console.WriteLine(lexer.CurrentLex + " - " + lexer.CurrentToken);
            } while (lexer.Read());
            Console.WriteLine(lexer.CurrentLex + " - " + lexer.CurrentToken);
        }
    }
}