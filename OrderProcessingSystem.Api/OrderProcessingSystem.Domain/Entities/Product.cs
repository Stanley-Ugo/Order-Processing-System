namespace OrderProcessingSystem.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
