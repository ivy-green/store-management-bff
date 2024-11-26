using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Interfaces;

namespace ProjectBase.Domain.Entities
{
    public class Product : EntityBase, ISoftDelete
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? Desc { get; set; }
        public string CreatorUsername { get; set; } = string.Empty;
        public virtual int ProductTypeCode { get; set; }
        public bool IsDeleted { get; set; }
        public Status Status { get; set; }
        public virtual User? Creator { get; set; }
        public virtual ProductType? ProductType { get; set; }
    }

    public enum Status
    {
        Available = 1,
        Disable = 2,
        OutOfStock = 3,
    }
}
