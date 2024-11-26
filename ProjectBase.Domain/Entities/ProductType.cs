using ProjectBase.Domain.Interfaces;

namespace ProjectBase.Domain.Entities
{
    public class ProductType : ISoftDelete
    {
        public int Code { get; set; }
        public DateTime CreateAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public virtual List<Product> Products { get; set; } = [];
        public bool IsDeleted { get; set; }
    }
}
