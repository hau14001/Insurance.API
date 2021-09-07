using System;

namespace Insurance.Domain.Common
{
    public abstract class AuditedEntityDto : AuditedEntityDto<Guid>
    {
    }

    public abstract class AuditedEntityDto<TKey> : EntityDto<TKey> where TKey : IEquatable<TKey>
    {
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}