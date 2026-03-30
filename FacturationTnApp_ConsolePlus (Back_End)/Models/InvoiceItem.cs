namespace FacturationTnApp.Models
{
    public class InvoiceItem
    {
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set; }

        public decimal LineTotalHT => Product.UnitPriceHT * Quantity;
        public decimal LineVAT => LineTotalHT * Product.VatRate / 100m;
        public decimal LineTotalTTC => LineTotalHT + LineVAT;

        public override string ToString()
        {
            return $"{Product.Name} x{Quantity} | HT {LineTotalHT:F3} | TVA {LineVAT:F3} | TTC {LineTotalTTC:F3}";
        }
    }
}
