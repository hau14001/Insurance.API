using System;

namespace Insurance.Domain.Common
{
    public abstract class AuditedEntity : AuditedEntity<Guid>
    {
    }

    public abstract class AuditedEntity<TKey> : Entity<TKey>, IAuditedEntity where TKey : IEquatable<TKey>
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}