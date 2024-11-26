namespace ProjectBase.Domain.Abstractions
{
    public class EntityBase
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateAt { get; set; }
    }
}
