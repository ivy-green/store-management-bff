namespace ProjectBase.Domain.DTOs.Requests
{
    public class ProductTypeCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
    }
    public class ProductTypeUpdateDTO
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
    }
}
