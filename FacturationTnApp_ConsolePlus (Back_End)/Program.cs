using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FacturationTnApp.Models;

namespace FacturationTnApp
{
    internal class Program
    {
        static List<Customer> customers = new List<Customer>();
        static List<Product> products = new List<Product>();
        static List<Invoice> invoices = new List<Invoice>();

        static int customerId = 1;
        static int productId = 1;
        static int invoiceId = 1;

        static readonly string dataFilePath = "appdata.json";

        static void Main(string[] args)
        {
            LoadOrSeed();

            bool running = true;

            while (running)
            {
                Console.Clear();
                ShowMainMenu();
                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCustomer();
                        break;
                    case "2":
                        DeleteCustomer();
                        break;
                    case "3":
                        AddProduct();
                        break;
                    case "4":
                        DeleteProduct();
                        break;
                    case "5":
                        CreateInvoice();
                        break;
                    case "6":
                        ShowInvoices(invoices, "INVOICE HISTORY");
                        break;
                    case "7":
                        SearchInvoicesByCustomer();
                        break;
                    case "8":
                        ShowTaxDashboard();
                        break;
                    case "9":
                        ShowSalesDashboard();
                        break;
                    case "10":
                        SaveData();
                        break;
                    case "11":
                        LoadData();
                        break;
                    case "0":
                        SaveData(false);
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine(" FACTURATION TN APP - MAIN MENU");
            Console.WriteLine("=================================================");
            Console.WriteLine("1. Add Customer");
            Console.WriteLine("2. Delete Customer");
            Console.WriteLine("3. Add Product");
            Console.WriteLine("4. Delete Product");
            Console.WriteLine("5. Create Invoice");
            Console.WriteLine("6. Show Invoices");
            Console.WriteLine("7. Search Invoices by Customer");
            Console.WriteLine("8. Tax Dashboard");
            Console.WriteLine("9. Sales Dashboard (Revenue by Month)");
            Console.WriteLine("10. Save Data to File");
            Console.WriteLine("11. Load Data from File");
            Console.WriteLine("0. Exit");
            Console.WriteLine("=================================================");
            Console.WriteLine($"Data file: {dataFilePath}");
            Console.WriteLine("=================================================");
        }

        static void LoadOrSeed()
        {
            AppData data = DataStore.Load(dataFilePath);
            customers = data.Customers;
            products = data.Products;
            invoices = data.Invoices;
            customerId = data.NextCustomerId;
            productId = data.NextProductId;
            invoiceId = data.NextInvoiceId;

            if (customers.Count == 0 && products.Count == 0 && invoices.Count == 0)
            {
                SeedData();
                SaveData(false);
            }
        }

        static void SeedData()
        {
            customers.Add(new Customer { Id = customerId++, Name = "Yasin Mokni", Phone = "20123456" });
            customers.Add(new Customer { Id = customerId++, Name = "Ali Ben Salah", Phone = "22111222" });

            products.Add(new Product { Id = productId++, Name = "Laptop", UnitPriceHT = 2500m, VatRate = 19m });
            products.Add(new Product { Id = productId++, Name = "Notebook Pack", UnitPriceHT = 12m, VatRate = 7m });
            products.Add(new Product { Id = productId++, Name = "Printer", UnitPriceHT = 480m, VatRate = 13m });
        }

        static void AddCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== ADD CUSTOMER ===");
            string name = ReadRequiredString("Customer name: ");
            string phone = ReadRequiredString("Phone: ");

            Customer customer = new Customer
            {
                Id = customerId++,
                Name = name,
                Phone = phone
            };

            customers.Add(customer);
            SaveData(false);
            Console.WriteLine("Customer added successfully.");
            Pause();
        }

        static void DeleteCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== DELETE CUSTOMER ===");

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers available.");
                Pause();
                return;
            }

            ShowCustomers();
            int id = ReadInt("Enter customer id to delete: ");
            Customer? customer = customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
            }
            else
            {
                customers.Remove(customer);
                SaveData(false);
                Console.WriteLine("Customer deleted successfully.");
                Console.WriteLine("Old invoices stay محفوظين because each invoice keeps its own customer snapshot.");
            }

            Pause();
        }

        static void AddProduct()
        {
            Console.Clear();
            Console.WriteLine("=== ADD PRODUCT ===");
            string name = ReadRequiredString("Product name: ");
            decimal unitPriceHT = ReadDecimal("Unit price HT: ");
            decimal vatRate = ReadDecimal("VAT rate (%): ");

            Product product = new Product
            {
                Id = productId++,
                Name = name,
                UnitPriceHT = unitPriceHT,
                VatRate = vatRate
            };

            products.Add(product);
            SaveData(false);
            Console.WriteLine("Product added successfully.");
            Pause();
        }

        static void DeleteProduct()
        {
            Console.Clear();
            Console.WriteLine("=== DELETE PRODUCT ===");

            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
                Pause();
                return;
            }

            ShowProducts();
            int id = ReadInt("Enter product id to delete: ");
            Product? product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                Console.WriteLine("Product not found.");
            }
            else
            {
                products.Remove(product);
                SaveData(false);
                Console.WriteLine("Product deleted successfully.");
                Console.WriteLine("Old invoices stay محفوظين because each invoice line keeps its own product snapshot.");
            }

            Pause();
        }

        static void CreateInvoice()
        {
            Console.Clear();
            Console.WriteLine("=== CREATE INVOICE ===");

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers available. Add a customer first.");
                Pause();
                return;
            }

            if (products.Count == 0)
            {
                Console.WriteLine("No products available. Add a product first.");
                Pause();
                return;
            }

            Customer selectedCustomer = ChooseCustomer();
            decimal taxStamp = ReadDecimal("Tax stamp amount: ");

            Invoice invoice = new Invoice
            {
                Id = invoiceId++,
                Customer = new Customer
                {
                    Id = selectedCustomer.Id,
                    Name = selectedCustomer.Name,
                    Phone = selectedCustomer.Phone
                },
                Date = DateTime.Now,
                TaxStamp = taxStamp
            };

            bool addMore = true;

            while (addMore)
            {
                Product selectedProduct = ChooseProduct();
                int quantity = ReadInt("Quantity: ");

                InvoiceItem item = new InvoiceItem
                {
                    Product = new Product
                    {
                        Id = selectedProduct.Id,
                        Name = selectedProduct.Name,
                        UnitPriceHT = selectedProduct.UnitPriceHT,
                        VatRate = selectedProduct.VatRate
                    },
                    Quantity = quantity
                };

                invoice.Items.Add(item);

                Console.Write("Add another product? (y/n): ");
                string? answer = Console.ReadLine();
                addMore = answer != null && answer.Trim().ToLower() == "y";
            }

            invoices.Add(invoice);
            SaveData(false);

            Console.WriteLine();
            Console.WriteLine("Invoice created successfully.");
            Console.WriteLine($"Total HT  : {invoice.TotalHT:F3}");
            Console.WriteLine($"Total VAT : {invoice.TotalVAT:F3}");
            Console.WriteLine($"Tax Stamp : {invoice.TaxStamp:F3}");
            Console.WriteLine($"Total TTC : {invoice.TotalTTC:F3}");
            Pause();
        }

        static void SearchInvoicesByCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== SEARCH INVOICES BY CUSTOMER ===");
            string keyword = ReadRequiredString("Enter customer name or part of it: ");

            List<Invoice> filtered = invoices
                .Where(i => i.Customer.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(i => i.Date)
                .ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine("No invoices found for this customer.");
                Pause();
                return;
            }

            ShowInvoices(filtered, $"RESULTS FOR CUSTOMER: {keyword}");
        }

        static Customer ChooseCustomer()
        {
            Console.WriteLine();
            ShowCustomers();

            while (true)
            {
                int id = ReadInt("Choose customer id: ");
                Customer? customer = customers.FirstOrDefault(c => c.Id == id);

                if (customer != null)
                {
                    return customer;
                }

                Console.WriteLine("Customer not found. Try again.");
            }
        }

        static Product ChooseProduct()
        {
            Console.WriteLine();
            ShowProducts();

            while (true)
            {
                int id = ReadInt("Choose product id: ");
                Product? product = products.FirstOrDefault(p => p.Id == id);

                if (product != null)
                {
                    return product;
                }

                Console.WriteLine("Product not found. Try again.");
            }
        }

        static void ShowCustomers()
        {
            Console.WriteLine("Customers:");
            foreach (Customer customer in customers)
            {
                Console.WriteLine($"{customer.Id}. {customer.Name} - {customer.Phone}");
            }
        }

        static void ShowProducts()
        {
            Console.WriteLine("Products:");
            foreach (Product product in products)
            {
                Console.WriteLine($"{product.Id}. {product.Name} | HT: {product.UnitPriceHT:F3} | VAT: {product.VatRate}%");
            }
        }

        static void ShowInvoices(List<Invoice> source, string title)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");

            if (source.Count == 0)
            {
                Console.WriteLine("No invoices found.");
                Pause();
                return;
            }

            foreach (Invoice invoice in source)
            {
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine($"Invoice ID : {invoice.Id}");
                Console.WriteLine($"Date       : {invoice.Date:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Customer   : {invoice.Customer.Name}");
                Console.WriteLine("Items:");

                foreach (InvoiceItem item in invoice.Items)
                {
                    Console.WriteLine($"- {item.Product.Name} | Qty: {item.Quantity} | HT: {item.LineTotalHT:F3} | VAT: {item.LineVAT:F3} | TTC: {item.LineTotalTTC:F3}");
                }

                Console.WriteLine($"Tax Stamp  : {invoice.TaxStamp:F3}");
                Console.WriteLine($"Total HT   : {invoice.TotalHT:F3}");
                Console.WriteLine($"Total VAT  : {invoice.TotalVAT:F3}");
                Console.WriteLine($"Total TTC  : {invoice.TotalTTC:F3}");
            }

            Pause();
        }

        static void ShowTaxDashboard()
        {
            Console.Clear();
            Console.WriteLine("=== TAX DASHBOARD ===");

            if (invoices.Count == 0)
            {
                Console.WriteLine("No invoices found.");
                Pause();
                return;
            }

            decimal totalVATCollected = invoices.Sum(i => i.TotalVAT);
            decimal totalTaxStamp = invoices.Sum(i => i.TaxStamp);

            Console.WriteLine($"Total VAT Collected : {totalVATCollected:F3}");
            Console.WriteLine($"Total Tax Stamp     : {totalTaxStamp:F3}");
            Console.WriteLine();
            Console.WriteLine("VAT by Rate:");

            var vatByRate = invoices
                .SelectMany(i => i.Items)
                .GroupBy(item => item.Product.VatRate)
                .OrderBy(group => group.Key);

            foreach (var group in vatByRate)
            {
                decimal vatAmount = group.Sum(item => item.LineVAT);
                Console.WriteLine($"- VAT {group.Key:F0}% : {vatAmount:F3}");
            }

            Pause();
        }

        static void ShowSalesDashboard()
        {
            Console.Clear();
            Console.WriteLine("=== SALES DASHBOARD ===");

            if (invoices.Count == 0)
            {
                Console.WriteLine("No invoices found.");
                Pause();
                return;
            }

            decimal totalRevenueHT = invoices.Sum(i => i.TotalHT);
            decimal totalRevenueTTC = invoices.Sum(i => i.TotalTTC);

            Console.WriteLine($"Total Revenue HT  : {totalRevenueHT:F3}");
            Console.WriteLine($"Total Revenue TTC : {totalRevenueTTC:F3}");
            Console.WriteLine();

            Console.WriteLine("Revenue by Month:");
            var revenueByMonth = invoices
                .GroupBy(i => i.Date.ToString("yyyy-MM"))
                .OrderBy(group => group.Key);

            foreach (var group in revenueByMonth)
            {
                Console.WriteLine($"- {group.Key} | HT: {group.Sum(i => i.TotalHT):F3} | TTC: {group.Sum(i => i.TotalTTC):F3}");
            }

            Console.WriteLine();
            Console.WriteLine("Revenue by Customer:");
            var revenueByCustomer = invoices
                .GroupBy(i => i.Customer.Name)
                .OrderBy(group => group.Key);

            foreach (var group in revenueByCustomer)
            {
                Console.WriteLine($"- {group.Key} | HT: {group.Sum(i => i.TotalHT):F3} | TTC: {group.Sum(i => i.TotalTTC):F3}");
            }

            Console.WriteLine();
            Console.WriteLine("Revenue by Product:");
            var revenueByProduct = invoices
                .SelectMany(i => i.Items)
                .GroupBy(item => item.Product.Name)
                .OrderBy(group => group.Key);

            foreach (var group in revenueByProduct)
            {
                Console.WriteLine($"- {group.Key} | HT: {group.Sum(item => item.LineTotalHT):F3} | TTC: {group.Sum(item => item.LineTotalTTC):F3}");
            }

            Pause();
        }

        static void SaveData(bool showMessage = true)
        {
            AppData data = new AppData
            {
                Customers = customers,
                Products = products,
                Invoices = invoices,
                NextCustomerId = customerId,
                NextProductId = productId,
                NextInvoiceId = invoiceId
            };

            DataStore.Save(dataFilePath, data);

            if (showMessage)
            {
                Console.WriteLine("Data saved successfully.");
                Pause();
            }
        }

        static void LoadData()
        {
            AppData data = DataStore.Load(dataFilePath);
            customers = data.Customers;
            products = data.Products;
            invoices = data.Invoices;
            customerId = data.NextCustomerId;
            productId = data.NextProductId;
            invoiceId = data.NextInvoiceId;

            Console.WriteLine("Data loaded successfully.");
            Pause();
        }

        static string ReadRequiredString(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }
                Console.WriteLine("Value cannot be empty. Try again.");
            }
        }

        static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter a valid positive integer.");
            }
        }

        static decimal ReadDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (input != null)
                {
                    input = input.Trim().Replace(',', '.');
                    if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) && value >= 0)
                    {
                        return value;
                    }
                }

                Console.WriteLine("Please enter a valid positive number.");
            }
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
