using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.DTOs.Responses
{
    public class ProductGetListDTOs
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public ProductTypeGetListDTOs? Type { get; set; }
        public Status Status { get; set; }
        public string? CreatorUsername { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Desc { get; set; }
        public bool? IsOnSale { get; set; } = null;
    }
}
