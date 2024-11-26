using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Interfaces;

namespace ProjectBase.Domain.Entities
{
    public class Coupon : EntityBase, ISoftDelete
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status => (DateTime.UtcNow <= StartDate) ? "Active" : "Inactive";
        public bool IsDeleted { get; set; }
    }
}
