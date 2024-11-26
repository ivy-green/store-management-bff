using ProjectBase.Domain.Abstractions;

namespace ProjectBase.Domain.Entities
{
    // to block token that have been logout 
    public class Blacklist : EntityBase
    {
        public string Token { get; set; } = string.Empty;
        public Action Action { get; set; }
    }

    public enum Action
    {
        Logout,
        Block,
    }
}
