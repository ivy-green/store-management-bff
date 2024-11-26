using ProjectBase.Jobs.Core.Entities;

namespace ProjectBase.Jobs.Core.DTOs.Requests
{
    public class StatisticBillMessageDTO : MessageDTO
    {
        public Bill? Bill { get; set; }
    }
}
