using System.Text.RegularExpressions;

namespace LL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StreamReader input = new("input.txt");
            StreamReader rules = new("rules.txt");
            StreamWriter output = new("output.txt");

            try
            {
                EXPPhars phars = new(input, rules);
                phars.Execute(1);
                Console.WriteLine("Входные данные успешно разобраны и соответствуют правилам");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            input.Close();
            output.Close();
        }
    }
}