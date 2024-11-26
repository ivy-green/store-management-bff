using Newtonsoft.Json;
using System.Web;

namespace ProjectBase.Domain.DTOs.Requests
{
    public class BillCreateDTO
    {
        public virtual string? Id { get; set; } = string.Empty;
        public virtual string Username { get; set; } = string.Empty;
        public string? CustomerUsername { get; set; }
        public string CustomerFullname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int? Status { get; set; }
        public double TotalPrice { get; set; }
        public double DiscountPrice { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public List<BillDetailsCreateDTO>? BillDetailsRequest { get; set; }
    }

    public class BillDetailsCreateDTO
    {
        public string? ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class BillFilter
    {
        public List<int> Status { get; set; } = [];

        public static bool TryParse(string val, out BillFilter data)
        {
            string decodedString = HttpUtility.UrlDecode(val);
            var res = JsonConvert.DeserializeObject<BillFilter>(decodedString);

            if (res is not null)
            {
                data = res;
                return true;
            }

            data = new BillFilter();
            return false;
        }
    }
}
