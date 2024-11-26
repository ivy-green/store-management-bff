namespace ProjectBase.Domain.DTOs.Requests
{
    public class ProductCreateDTO
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? Desc { get; set; }
        public int ProductTypeCode { get; set; }
    }
}
