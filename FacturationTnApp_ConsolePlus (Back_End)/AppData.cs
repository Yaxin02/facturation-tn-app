using System.Collections.Generic;
using FacturationTnApp.Models;

namespace FacturationTnApp
{
    public class AppData
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public int NextCustomerId { get; set; } = 1;
        public int NextProductId { get; set; } = 1;
        public int NextInvoiceId { get; set; } = 1;
    }
}
