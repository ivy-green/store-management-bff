namespace ProjectBase.Domain.Entities
{
    public class ProductOnSale
    {
        public string ProductId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public bool IsOnSale { get; set; } = false;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
