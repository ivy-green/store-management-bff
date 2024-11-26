namespace ProjectBase.Domain.Entities
{
    public class Role
    {
        public int Code { get; set; }
        public DateTime CreateAt { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
