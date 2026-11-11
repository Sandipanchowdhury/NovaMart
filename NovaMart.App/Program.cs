using System.Text;

namespace NovaMart.App;

internal class Program
{
    private const decimal GSTRate = 0.18m;
    private const int MaxCartLines =  10;

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Catalog: parallel arrays, so index i describes product i
        string[] names = {"Laptop", "Mobile" ,"Watch","headphones","keyboard"};
        decimal[] prices = {50000m, 20000m, 10000m, 2500m, 1200m};
        int[] stocks= {8, 25, 60 , 5, 0};

        // Cart: parallel arrays with a fixed capacity
        string[] cartNames = new string[MaxCartLines];
        decimal[] cartPrices = new decimal[MaxCartLines];
        int[] cartQuantities = new int[MaxCartLines];
        int cartCount = 0;


        Console.WriteLine("=== NovaMart ===");
        PrintCatalog(names, prices, stocks);

        while(cartCount < MaxCartLines)
        {
            Console.WriteLine();
            string search = ReadText("Product name (or 'done' to finish): ");

            if(search.Equals("done", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(search))
            {
                Console.WriteLine("Name cannot be empty.");
                continue;
            }


            int index = FindProductIndex(names , search);
            if (index == -1)
            {
                Console.WriteLine($"No product named '{search}' . Check the catalog above .");
                continue;
            }

            Console.WriteLine($"{names[index]}: {FormatMoney(prices[index])} -" + 
                              $"{GetStockStatus(stocks[index])} ({stocks[index]} left)");


            int? entered = ReadPositiveInt("Quantity");
            if(entered is null)
            {
                Console.WriteLine("Too many invalid attempts. Item skipped . ");
                continue;
            }
            int quantity = entered.Value;

            if (quantity > stocks[index])
            {
                Console.WriteLine($"Only {stocks[index]} available. item not added.");
                continue;
            }

            cartNames[cartCount] = names[index];
            cartPrices[cartCount] = prices[index];
            cartQuantities[cartCount] = quantity;
            cartCount++;

            stocks[index] -= quantity;
            Console.WriteLine($"Added {quantity} X { names[index]} to the cart.");
        }

        if(cartCount == MaxCartLines)
        {
            Console.WriteLine($"\nCart is Full ({MaxCartLines}) lines).");
        }
        PrintBill(cartNames, cartPrices, cartQuantities, cartCount);

    }


    // --------------- Array helpers -------------

    private static int FindProductIndex(string[] names, string search)
    {
        for (int i = 0 ;i< names.Length; i++)
        {
            if(names[i].Equals(search, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1; // -1 means "not found"
    }

    // ------------- Input helpers ------------

    private static string  ReadText(string prompt)
    {
        Console.Write(prompt);
        return(Console.ReadLine() ?? "").Trim();
    }

    private static int? ReadPositiveInt(string label, int  maxAttempts = 3)
    {
        for ( int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Console.Write($"{label} (attempt {attempt}/{maxAttempts}): ");
            if(int.TryParse(Console.ReadLine(), out int value) && value > 0)
            {
                return value;
            }
            Console.WriteLine($"{label} must be a whole number greater than 0.");
        }
        return null;
    }

    //---------------- Business rules ---------------

    private static string GetStockStatus(int stock) => stock switch
    {
        0 =>"Out of stock",
        <= 10 => "Low stock",
        _=> " In stock"
    };

    private static decimal GetDiscountRate(int quantity) => quantity switch
    {
        >= 50 => 0.15m,
        >= 10 => 0.10m,
        _ => 0m
    };

    private static decimal CalculateGst(decimal taxableAmount) => taxableAmount *GSTRate;


    // ----------------- Output ---------------------

    private static string FormatMoney(decimal amount) => $"₹{amount:N0}";

    private static void PrintCatalog(string[] names, decimal[] prices , int[] stocks)
    {
        Console.WriteLine();
        Console.WriteLine($"{"#", -4}{"Product" , -14} {"Price (₹)", 12}{"Stock", 8} Status");
        Console.WriteLine(new string('-', 50 ));

        for (int i =0 ; i< names.Length; i++)
        {
            Console.WriteLine($"{i + 1,-4}{names[i],-14}{prices[i],12:N2}{stocks[i],8}  {GetStockStatus(stocks[i])}");

        }

    }

    private static void PrintBill(string[] names, decimal[] prices, int[] quantities, int lineCount)
    {
        if (lineCount == 0)
        {
            Console.WriteLine("\nYour cart is empty.");
            return;
        }

        decimal subtotal = 0m;
        decimal discount = 0m;

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(new string('=', 50));
        sb.AppendLine($"{"Item",-14}{"Qty",5}{"Price",12}{"Total",14}");
        sb.AppendLine(new string('-', 50));

        for (int i = 0; i < lineCount; i++)
        {
            decimal lineTotal = prices[i] * quantities[i];
            subtotal += lineTotal;
            discount += lineTotal * GetDiscountRate(quantities[i]);

            sb.AppendLine($"{names[i],-14}{quantities[i],5}{prices[i],12:N2}{lineTotal,14:N2}");
        }

        decimal taxable = subtotal - discount;
        decimal gst = CalculateGst(taxable);
        decimal grandTotal = taxable + gst;

        sb.AppendLine(new string('-', 50));
        sb.AppendLine($"{"Subtotal",-31}{FormatMoney(subtotal),19}");
        sb.AppendLine($"{"Discount",-31}{"-" + FormatMoney(discount),19}");
        sb.AppendLine($"{"GST (18%)",-31}{FormatMoney(gst),19}");
        sb.AppendLine($"{"Grand total",-31}{FormatMoney(grandTotal),19}");
        sb.AppendLine(new string('=', 50));

        Console.Write(sb.ToString());

    }



}