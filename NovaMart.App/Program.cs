using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Novamart.App;

internal class Program
{
    private const decimal GstRate = 0.18m;
    private const int MaxProducts = 20;
    private const int MaxCartLines = 10;

    //Catalog (parallel arrays) and its usual slot count
    private static readonly string[] _names = new string[MaxProducts];
    private static readonly decimal[] _prices = new decimal[MaxProducts];
    private static readonly int[] _stocks = new int[MaxProducts];
    private static int _productCount;

    //cart (parallel arrays) and its used-slot count
    private static readonly string[] _cartNames = new string[MaxCartLines];
    private static readonly decimal[] _cartPrices = new decimal[MaxCartLines];
    private static readonly int[] _cartQuantities = new int[MaxCartLines];
    private static int _cartCount;


    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        SeedCatalog();

        bool running = true;
        while (running)
        {
            ShowMenu();
            int? choice = ReadMenuChoice();

            switch (choice)
            {
                case 1: ViewCatalog(); break;
                case 2: AddProduct(); break;
                case 3: SearchProducts(); break;
                case 4: UpdateStock(); break;
                case 5: AddToCart(); break;
                case 6: Checkout(); break;
                case 0: running = false; break;
                default: Console.WriteLine("Invalid option. Choose a number from 0 to 6."); break;

            }
        }

        Console.WriteLine("Thank you for using Novamart. See you Again");

    }

    // ====================== Menu ==================

        private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("========== NovaMart ==========");
        Console.WriteLine(" 1. View catalog");
        Console.WriteLine(" 2. Add product");
        Console.WriteLine(" 3. Search products");
        Console.WriteLine(" 4. Update stock");
        Console.WriteLine(" 5. Add to cart");
        Console.WriteLine(" 6. Checkout");
        Console.WriteLine(" 0. Exit");
        Console.WriteLine("==============================");
    }

    private static int? ReadMenuChoice()
    {
        Console.Write("Choose an option ");
        return int.TryParse(Console.ReadLine(), out int choice) ? choice : null ;
    }

    // ================= Catalog actions ==================

    private static void SeedCatalog()
    {
        AddProductRecord("Laptop", 50000m, 8);
        AddProductRecord("Mobile", 20000m, 25);
        AddProductRecord("Watch", 10000m, 60);
        AddProductRecord("Headphones", 2500m, 5);
        AddProductRecord("Keyboard", 1200m, 0);
    }

    private static void AddProductRecord(string name , decimal price , int stock)
    {
        _names[_productCount] = name;
        _prices[_productCount] = price;
        _stocks[_productCount] = stock;
        _productCount++;
    }

    private static void ViewCatalog()
    {
        Console.WriteLine();
        Console.WriteLine($"{"#", -4}{"Product", -14}{"Price ()", 12}{"Stock" , 8} , Status");
        Console.WriteLine(new string('-', 50));

        for ( int i = 0; i< _productCount; i++)
        {
            Console.WriteLine($"{i+1, -4}{_names[i], -14}{_prices[i], 12:N2}{_stocks[i], 8} {GetStockStatus(_stocks[i])}");
        }
        
    }

    private static void AddProduct()
    {
        if(_productCount == MaxProducts)
        {
            Console.WriteLine("Catalog is full.");
            return;
        }

        string name = ReadText("Product name: ");
        if(name.Length == 0)
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        if(FindProductIndex(name) != -1)
        {
            Console.WriteLine($"'{name}' already exists.");
            return;
        }

        decimal price = ReadPositiveDecimal("Unit price (₹): ");
        int stock = ReadInt("Opening stock: ", min: 0);

        AddProductRecord(name, price, stock);
        Console.WriteLine($"'{name}' added to the catalog.");
    }

    private static void SearchProducts()
    {
        string term = ReadText("Search for: ");
        if(term.Length == 0)
        {
            Console.WriteLine("Enter some text to search.");
            return;
        }

        int matches = 0;
        for (int i = 0; i < _productCount; i++)
        {
            if(_names[i].Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"{_names[i], -4}{FormatMoney(_prices[i]), 14}   stock: {_stocks[i]}");
                matches++;
            }
        }

        if (matches == 0)
        {
            Console.WriteLine("No matching products.");
        }
    }

    private static void UpdateStock()
    {
        string name = ReadText("Product name: ");
        int index = FindProductIndex(name);
        if (index == -1)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        _stocks[index] = ReadInt($"New stock for{_names[index]} (currently {_stocks[index]}): ", min: 0);
        Console.WriteLine("Stock updated.");

    }

    //================ Cart Actions ===============

    private static void AddToCart()
    {
        string name = ReadText("Product name: ");
        int index = FindProductIndex(name);
        if (index == -1)
        {
            Console.WriteLine("Product not found. Check the catalog.");
            return;
        }

        Console.WriteLine($"{_names[index]}: {FormatMoney(_prices[index])} - "+
                          $"{GetStockStatus(_stocks[index])} ({_stocks[index]} left)");

        int? entered = ReadPositiveInt("quantity");
        if (entered is null)
        {
            Console.WriteLine("Too many invalid attempts. Item skipped.");
            return;
        }

        int quantity = entered.Value;

        if (quantity > _stocks[index])
        {
            Console.WriteLine($"Only {_stocks[index]} available. Item not added.");
            return;
        }

        int line = FindCartLine(_names[index]);
        if(line >= 0)
        {
            _cartQuantities[line] += quantity;
        }
        else
        {
            if(_cartCount == MaxCartLines)
            {
                Console.WriteLine("Cart is full.");
                return;
            }

            _cartNames[_cartCount] = _names[index];
            _cartPrices[_cartCount] = _prices[index];
            _cartQuantities[_cartCount] = quantity;
            _cartCount++;
        }

        _stocks[index] -= quantity;
        Console.WriteLine($"Added {quantity} X {_names[index]} to the cart.");
    }

    private static void Checkout()
    {
        if(_cartCount == 0)
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }

        PrintBill();
        _cartCount = 0;
        Console.WriteLine("Order placed. Thank you!");
    }

    // ================ Lookups ==================

    private static int FindProductIndex(string name)
    {
        for (int i = 0; i < _productCount; i++)
        {
            if(_names[i].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            
            }
        }
        return -1;
    }

    private static int FindCartLine(string name)
    {
        for (int i = 0; i < _cartCount; i++)
        {
            if(_cartNames[i].Equals(name , StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }


    // ================= Input helpers ==============

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
    
    private static int ReadInt(string prompt, int min)
    {
        while (true)
        {
            Console.Write(prompt);
            if(int.TryParse(Console.ReadLine(), out int value) && value >= min)
            {
                return value;
            }
            Console.WriteLine($"Enter a whole number of {min} or more");
        }
    }

    private static int? ReadPositiveInt(string lable, int maxAttempts = 3)
    {
        for ( int attempt = 1; attempt<= maxAttempts; attempt++)
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

    // ============== Business rules ================

    private static string GetStockStatus(int stock) => stock switch
    {
        0=> " Out of Stock",
        <= 10 => "Low stock",
        _ => "In stock"
    };

    private static decimal GetDiscountRate(int quantity) => quantity switch
    {
        >= 50 => 0.15m,
        >= 10 => 0.10m,
        _ => 0m
    };

    private static decimal CalculateGst(decimal taxableAmount) => taxableAmount * GstRate;

    // ================== Output ==========================
    private static string FormatMoney(decimal amount) => $"₹{amount:N2}";

    private static void PrintBill()
    {
        decimal subtotal = 0m;
        decimal discount = 0m;

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(new string('=', 50));
        sb.AppendLine($"{"Item",-14}{"Qty",5}{"Price",12}{"Total",14}");
        sb.AppendLine(new string('-', 50));

        for (int i = 0; i < _cartCount; i++)
        {
            decimal lineTotal = _cartPrices[i] * _cartQuantities[i];
            subtotal += lineTotal;
            discount += lineTotal * GetDiscountRate(_cartQuantities[i]);

            sb.AppendLine($"{_cartNames[i],-14}{_cartQuantities[i],5}{_cartPrices[i],12:N2}{lineTotal,14:N2}");
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