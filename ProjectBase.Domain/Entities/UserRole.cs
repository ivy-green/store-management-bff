namespace ProjectBase.Domain.Entities
{
    public class UserRole
    {
        public virtual string Username { get; set; } = string.Empty;
        public virtual int RoleCode { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Role? Role { get; set; }

    }
}
