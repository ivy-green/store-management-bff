using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Interfaces;

namespace ProjectBase.Domain.Entities
{
    public class Branch : EntityBase, ISoftDelete
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public virtual List<User> Users { get; set; } = [];
    }
}
