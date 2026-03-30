namespace FacturationTnApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal UnitPriceHT { get; set; }
        public decimal VatRate { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Name} | HT {UnitPriceHT:F3} | TVA {VatRate:F0}%";
        }
    }
}
