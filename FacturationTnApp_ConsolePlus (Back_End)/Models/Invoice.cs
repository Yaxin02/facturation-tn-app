using System;
using System.Collections.Generic;
using System.Linq;

namespace FacturationTnApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public Customer Customer { get; set; } = new Customer();
        public DateTime Date { get; set; } = DateTime.Now;
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public decimal TaxStamp { get; set; }

        public decimal TotalHT => Items.Sum(i => i.LineTotalHT);
        public decimal TotalVAT => Items.Sum(i => i.LineVAT);
        public decimal TotalTTC => TotalHT + TotalVAT + TaxStamp;

        public override string ToString()
        {
            return $"Invoice #{Id} - {Customer.Name} - {Date:yyyy-MM-dd} - TTC {TotalTTC:F3}";
        }
    }
}
