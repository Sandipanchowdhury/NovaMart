namespace NovaMart.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("====================");
        Console.WriteLine("Welcome to NovaMart");
        Console.WriteLine("====================");
        

        //Step : 1 -------------------------------------  
        // Console.Write("Rente your Name: ");
        // string? _name = Console.ReadLine();
        
        // if (_name == "")
        // {
        //      Console.WriteLine($"Hello, User! Lets's build something great");
        // }
        // else
        // {
        //     Console.WriteLine($"Hello, {name}! Lets's build something great");

        // }


        // Step2 -----------------------------------------
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        const decimal GstRate = 0.18m;
        Console.WriteLine("=== NovaMart: Add Product ===");

        Console.Write("Product name:");
        string name = Console.ReadLine() ?? "Unknown";

        Console.Write("Unit price (₹): ");
        decimal unitPrice = decimal.Parse(Console.ReadLine()!);

        Console.Write("Quantity: ");
        int quantity = int.Parse(Console.ReadLine()!);

        decimal subtotal = unitPrice * quantity;
        decimal gst = subtotal * GstRate;
        decimal total = subtotal + gst;
        bool isBulkOrder = quantity >= 10;


        Console.WriteLine();
        Console.WriteLine($"Product  : {name}");
        Console.WriteLine($"Subtotal : ₹{subtotal:N2}");
        Console.WriteLine($"GST (18%): ₹{gst:N2}");
        Console.WriteLine($"Total    : ₹{total:N2}");
        Console.WriteLine($"Bulk order? {isBulkOrder}");


    }
}
