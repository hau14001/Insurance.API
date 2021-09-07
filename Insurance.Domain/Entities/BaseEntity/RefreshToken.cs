using Insurance.Domain.Common;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class RefreshToken : Entity
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public Guid UserId { get; set; }

        public string RemoteIpAddress { get; set; }

        public bool Active => DateTime.UtcNow <= this.Expires;

        public void Deactivate() => Expires = DateTime.UtcNow;
    }
}