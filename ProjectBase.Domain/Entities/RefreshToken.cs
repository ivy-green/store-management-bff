namespace ProjectBase.Domain.Entities
{
    public class RefreshToken
    {
        public string Username { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public int Duration { get; set; }
    }
}
