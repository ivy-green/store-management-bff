using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.DTOs.Message
{
    public class StatisticBillMessageDTO : MessageDTO
    {
        public Bill? Bill { get; set; }
    }
}
