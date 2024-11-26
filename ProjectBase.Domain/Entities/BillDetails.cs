namespace ProjectBase.Domain.Entities
{
    public class BillDetails
    {
        public virtual string BillId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice => Price * Quantity;
    }
}
