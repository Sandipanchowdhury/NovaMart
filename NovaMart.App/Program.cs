namespace NovaMart.App;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        const decimal GSTRate = 0.18m;
        decimal cartSubtotal = 0m;
        decimal cartDiscount = 0m;
        int itemsAdded =0;

        Console.WriteLine("===NovaMart: Build your Cart ===");

        while (true)
        {
            Console.WriteLine();
            Console.Write("Product name(or 'done' to finish): ");
            string  name = (Console.ReadLine() ?? "").Trim();

            if(name.Equals("done" , StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            
            if(name.Length == 0)
            {
                Console.WriteLine("Name cannot be empty.");
                continue;
            }

            decimal unitPrice;

            
            while (true)
            {
                Console.Write("Unit price (₹): ");
                if (decimal.TryParse(Console.ReadLine(), out unitPrice) && unitPrice> 0)
                {
                    break;
                }
                Console.WriteLine("Enter a valid price greater than 0.");
            }

{            // Give the user only 3 attempts for quantity
            int quantity = 0;
            bool quantityValid = false;

            for(int attempt = 1; attempt <= 3; attempt++)
            {
                Console.Write($"Quantity (attempt {attempt}/3):");
                if(int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
                {
                    quantityValid = true;
                    break;
                }
                    Console.WriteLine("Quantity must be a whole number greater than 0.");
            }

            if (!quantityValid)
            {
                Console.WriteLine("Too many invalid attemps. Item Skipped");
                continue;
            }

            Console.Write("Stock available: ");
            int stock = int.TryParse(Console.ReadLine(), out int parsedStock) && parsedStock >= 0 ? parsedStock : 0;

            string stockStatus = stock switch
            {
                0 => "Out of stock",
                <= 10 => "Low stock",
                _=>"In Stock"
            };
            Console.WriteLine($"Stock status: {stockStatus}");

            if (quantity > stock)
            {
                Console.WriteLine($"Only {stock} available. Item not added.");
                continue;
            }

            decimal discountRate = quantity switch
            {
                >= 50 =>0.15m,
                >= 10 => 0.10m,
                _=> 0m
            };
            
            
            decimal lineSubtotal = unitPrice * quantity;
            decimal lineDiscount = lineSubtotal * discountRate;

            cartSubtotal +=  lineSubtotal;
            cartDiscount += lineDiscount;
            itemsAdded++;

            Console.WriteLine($"Added {quantity} X {name} = ₹{lineSubtotal - lineDiscount:N2}"+$"(discount {discountRate:P0})");

            if (itemsAdded == 0)
            {
                Console.WriteLine("\nYour cart is empty.");
                return;
            }

            decimal taxable = cartSubtotal - cartDiscount;
            decimal gst = taxable * GSTRate;
            decimal grandTotal = taxable + gst;

            Console.WriteLine();
            Console.WriteLine(new string('=', 36));
            Console.WriteLine($"Items added : {itemsAdded}");
            Console.WriteLine($"Subtotal    : ₹{cartSubtotal:N2}");
            Console.WriteLine($"Discount    : -₹{cartDiscount:N2}");
            Console.WriteLine($"GST (18%)   : ₹{gst:N2}");
            Console.WriteLine($"Grand total : ₹{grandTotal:N2}");
            Console.WriteLine(new string('=', 36));


        }
        }

    }
}
