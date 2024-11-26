using ProjectBase.Jobs.Core.Abstractions;
using ProjectBase.Jobs.Core.Enums;

namespace ProjectBase.Jobs.Core.Entities
{
    public class Bill : EntityBase
    {
        public virtual string Username { get; private set; } = string.Empty;
        public double TotalPrice { get; private set; }
        public double DiscountPrice { get; private set; }
        public BillStatus Status { get; set; } = BillStatus.New;
        //public virtual User? User { get; private set; } = null;
        public virtual List<BillDetails> BillDetails { get; set; } = [];
    }
}
