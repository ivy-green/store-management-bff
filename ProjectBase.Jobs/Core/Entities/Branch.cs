using ProjectBase.Jobs.Core.Abstractions;
using ProjectBase.Jobs.Core.Interfaces.IEntity;

namespace ProjectBase.Jobs.Core.Entities
{
    public class Branch : EntityBase, ISoftDelete
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        //public virtual List<User> Users { get; set; } = [];
        public bool IsDeleted { get; set; }
    }
}
