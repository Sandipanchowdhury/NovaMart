namespace NovaMart.App;

internal class Program
{
    private const decimal GSTRate = 0.18m;

    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        decimal cartSubtotal = 0m;
        decimal cartDiscount = 0m;
        int itemsAdded = 0;


        Console.WriteLine("=== NovaMart: Build Your Cart ===");

        while (true)
        {
            Console.WriteLine();
            string name = ReadText("Product name( or 'done' to finish): ");

            if (name.Equals("done", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if(name.Length == 0)
            {
                Console.WriteLine("name cannot be empty.");
                continue ;           
            }

            decimal unitPrice = ReadPositiveDecimal("Unit price (₹): ");

            int? enteredQuantity = ReadPositiveInt("Quantity");
            if(enteredQuantity is null)
            {
                Console.WriteLine("Too many invalid attepms. Item skipped.");
                continue;
            }
            int quantity = enteredQuantity.Value;

            int stock = ReadStock();
            Console.WriteLine($"Stock status: {GetStockStatus(stock)}");

            if (quantity > stock)
            {
                Console.WriteLine($"Only {stock} available. Item not added. ");
                continue;
            }

            decimal discountrate = GetDiscountRate(quantity);
            decimal lineSubtotal = unitPrice * quantity;
            decimal lineDiscount = lineSubtotal * discountrate;

            cartSubtotal += lineSubtotal;
            cartDiscount += lineDiscount;
            itemsAdded ++;

            Console.WriteLine($"added {quantity} x {name} = {FormatMoney(lineSubtotal -  lineDiscount)}" + $"(discount {discountrate:P0})");

        }
        PrintBill(cartSubtotal , cartDiscount, itemsAdded );
    }


    // ---------------- Input helpers ---------------------

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? "").Trim();

    }

    private static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if(decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
            {
                return value;
            }
            Console.WriteLine("Enter a valid number greater than 0.");
        }
    }

    private static int? ReadPositiveInt(string lable, int maxAttempts = 3)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Console.Write($"{lable} (attempt {attempt}/{maxAttempts}): ");
            if(int.TryParse(Console.ReadLine(), out int value) && value > 0)
            {
                return value;
            }
            Console.WriteLine($"{lable} must be a whole number greater than 0.");
        }
        return null;
    }

    private static int ReadStock()
    {
        Console.Write("Stock available: ");
        return int.TryParse(Console.ReadLine(), out int stock) && stock >= 0 ? stock : 0;

    }


    // -------------- Business rules ----------------

    private static string GetStockStatus(int stock) => stock switch
    {
        0 => "Out of stock",
        <= 10 => "Low stock",
        _=> "In stock"
    };

    private static decimal GetDiscountRate(int quantity) => quantity switch
    {
        >= 50 => 0.15m,
        >= 10 => 0.10m,
        _=> 0m
    };

    private static decimal CalculateGst(decimal taxableAmount) => taxableAmount * GSTRate;

    //-------------------- Output -------------------
    private static string FormatMoney(decimal amount) => $"₹{amount:N2}";

    private static void PrintBill(decimal subtotal , decimal discount, int itemsAdded)
    {
        if(itemsAdded == 0)
        {
            Console.WriteLine("\nYour cart is empty.");
            return;
        }

        decimal taxable = subtotal - discount;
        decimal gst = CalculateGst(taxable);
        decimal grandTotal = taxable + gst;

        Console.WriteLine();
        Console.WriteLine(new string('=', 36));
        Console.WriteLine($"Items added : {itemsAdded}");
        Console.WriteLine($"Subtotal    : {FormatMoney(subtotal)}");
        Console.WriteLine($"Discount    : -{FormatMoney(discount)}");
        Console.WriteLine($"GST (18%)   : {FormatMoney(gst)}");
        Console.WriteLine($"Grand total : {FormatMoney(grandTotal)}");
        Console.WriteLine(new string('=', 36));
    }
    
}
