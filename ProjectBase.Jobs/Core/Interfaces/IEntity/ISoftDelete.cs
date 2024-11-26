namespace ProjectBase.Jobs.Core.Interfaces.IEntity
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
