using Insurance.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class User : IdentityUser<Guid>, IEntity<Guid>, IAuditedEntity, ISoftDelete
    {
        public string Code { get; set; }
        public string BPassword { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsLock { get; set; } = false;
        public DateTime? ExpiredDate { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public string Ip { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public bool HasValidRefreshToken(string refreshToken) => RefreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);

        public void AddRefreshToken(string token, string remoteIpAddress, double daysToExpire = 7.0) => RefreshTokens.Add(new RefreshToken()
        {
            Token = token,
            Expires = DateTime.UtcNow.AddDays(daysToExpire),
            UserId = Id,
            RemoteIpAddress = remoteIpAddress
        });

        public void DeactiveActiveRefreshTokens()
        {
            foreach (RefreshToken refreshToken in RefreshTokens.Where(t => t.Active))
                refreshToken.Deactivate();
        }

        public void DeactivateRefreshToken(string refreshToken) => RefreshTokens.First(t => t.Token == refreshToken).Deactivate();

        public void DeactivateRefreshToken()
        {
            foreach (RefreshToken refreshToken in RefreshTokens.Where(t => t.Active))
                refreshToken.Deactivate();
        }

        public User()
        {
        }
    }
}