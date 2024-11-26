using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.Enums;

namespace ProjectBase.Domain.Entities
{
    public class Bill : EntityBase
    {
        public double TotalPrice { get; private set; }
        public double DiscountPrice { get; private set; }
        public BillStatus Status { get; set; } = BillStatus.New;

        public virtual string? ShipperUsername { get; set; } = null;
        public virtual User? Shipper { get; private set; } = null;

        // user create
        public virtual string Username { get; private set; } = string.Empty;
        public virtual User? User { get; private set; } = null;

        // customer's information
        public virtual string? CustomerUsername { get; private set; } = string.Empty;
        public virtual string CustomerFullname { get; private set; } = string.Empty;
        public virtual string Address { get; private set; } = string.Empty;
        public virtual string PhoneNumber { get; private set; } = string.Empty;
        public virtual string? Note { get; private set; } = string.Empty;

        public string? Log { get; set; } = string.Empty;
        public virtual List<BillDetails> BillDetails { get; set; } = [];
    }
}
