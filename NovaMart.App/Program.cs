namespace NovaMart.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("====================");
        Console.WriteLine("Welcome to NovaMart");
        Console.WriteLine("====================");

        Console.Write("Rente your Name: ");
        string? name = Console.ReadLine();
        
        if (name == "")
        {
             Console.WriteLine($"Hello, User! Lets's build something great");
        }
        else
        {
            Console.WriteLine($"Hello, {name}! Lets's build something great");
            
        }

    }
}
