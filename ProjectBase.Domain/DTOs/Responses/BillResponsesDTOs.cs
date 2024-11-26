using ProjectBase.Domain.DTOs.Models;
using ProjectBase.Domain.Enums;

namespace ProjectBase.Domain.DTOs.Responses
{
    public class BillGetListDTO
    {
        public virtual string? Id { get; set; }
        public virtual string Username { get; set; } = string.Empty;
        public BillStatus Status { get; set; }
        public double TotalPrice { get; set; }
        public double DiscountPrice { get; set; }
        public DateTime CreateAt { get; set; }
        public List<BillDetailsGetListDTO>? BillDetailsRequest { get; set; }
        public List<BillLogger> TrackingLogger { get; set; } = [];
    }

    public class BillDetailsGetListDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
