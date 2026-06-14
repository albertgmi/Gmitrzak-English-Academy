namespace inzBackend.Entities.Base
{
    public abstract class AuditableEntity : BaseEntity
    {
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
