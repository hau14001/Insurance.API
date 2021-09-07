using System;

namespace Insurance.Domain.Common
{
    public abstract class EntityDto : EntityDto<Guid>
    {
    }

    public abstract class EntityDto<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
}