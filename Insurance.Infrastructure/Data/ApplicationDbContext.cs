using System;
using System.Collections.Generic;
using Insurance.Domain.Common;
using Insurance.Domain.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<BaseFileEntity> BaseFileEntity { get; set; }

        //Custom

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<RoleClaim>().ToTable("RoleClaims");
            builder.Entity<UserToken>().ToTable("UserTokens");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<RefreshToken>().ToTable("RefreshTokens");
            builder.Entity<UserPermission>().ToTable("UserPermissions");
            builder.Entity<RolePermission>().ToTable("RolePermissions");

            builder.Entity<User>().HasData(
          new User
          {
              Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f475f"),
              UserName = "haudts@gmail.com",
              NormalizedUserName = "HAUDTS@GMAIL.COM",
              Email = "haudts@gmail.com",
              NormalizedEmail = "HAUDTS@GMAIL.COM",
              IsSystemUser = true,
              PhoneNumber = "0979651347",
              PhoneNumberConfirmed = true,
              EmailConfirmed = true,
              CreatedAt = new DateTime(2021, 4, 4),
              UpdatedAt = new DateTime(2021, 4, 4),
              CreatedBy = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f475f"),
              UpdatedBy = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f475f"),
              SecurityStamp = "0b01f8df-61df-4f41-a95f-a858e83f475f",
              FullName = "Tran Van Hau",
              BPassword = BCrypt.Net.BCrypt.HashPassword("Hau123@")
          }
  );
            //builder.Entity<Permission>().HasData(
            //    new Permission
            //    {
            //        Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4001"),
            //        DisplayName = PermissionConstants.Manager,
            //        Name = PermissionConstants.Manager,
            //    },
            //    new Permission
            //    {
            //        Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4002"),
            //        DisplayName = PermissionConstants.Leader,
            //        Name = PermissionConstants.Leader,
            //    },
            //    new Permission
            //    {
            //        Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4003"),
            //        DisplayName = PermissionConstants.Qs,
            //        Name = PermissionConstants.Qs,
            //    }
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4004"),
            //    DisplayName = PermissionConstants.ReadUser,
            //    Name = PermissionConstants.ReadUser,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4005"),
            //    DisplayName = PermissionConstants.UpdateUser,
            //    Name = PermissionConstants.UpdateUser,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4006"),
            //    DisplayName = PermissionConstants.ManageUser,
            //    Name = PermissionConstants.ManageUser,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4007"),
            //    DisplayName = PermissionConstants.ManageRole,
            //    Name = PermissionConstants.ManageRole,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4008"),
            //    DisplayName = PermissionConstants.UpdateRole,
            //    Name = PermissionConstants.UpdateRole,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4009"),
            //    DisplayName = PermissionConstants.CreateRole,
            //    Name = PermissionConstants.CreateRole,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4010"),
            //    DisplayName = PermissionConstants.ManageCategory,
            //    Name = PermissionConstants.ManageCategory,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4011"),
            //    DisplayName = PermissionConstants.ManageCommand,
            //    Name = PermissionConstants.ManageCommand,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4012"),
            //    DisplayName = PermissionConstants.ManageCourseChapter,
            //    Name = PermissionConstants.ManageCourseChapter,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4013"),
            //    DisplayName = PermissionConstants.ManageCourse,
            //    Name = PermissionConstants.ManageCourse,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4014"),
            //    DisplayName = PermissionConstants.ManageCourseLesson,
            //    Name = PermissionConstants.ManageCourseLesson,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4015"),
            //    DisplayName = PermissionConstants.DeleteCourse,
            //    Name = PermissionConstants.DeleteCourse,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4016"),
            //    DisplayName = PermissionConstants.ManagePost,
            //    Name = PermissionConstants.ManagePost,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4017"),
            //    DisplayName = PermissionConstants.ManageRevitToolVersion,
            //    Name = PermissionConstants.ManageRevitToolVersion,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4018"),
            //    DisplayName = PermissionConstants.ManageToolProduct,
            //    Name = PermissionConstants.ManageToolProduct,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4019"),
            //    DisplayName = PermissionConstants.ManageTrainer,
            //    Name = PermissionConstants.ManageTrainer,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4020"),
            //    DisplayName = PermissionConstants.ManageUserToolLicense,
            //    Name = PermissionConstants.ManageUserToolLicense,
            //},
            //new Permission
            //{
            //    Id = Guid.Parse("0b01f8df-61df-4f41-a95f-a858e83f4021"),
            //    DisplayName = PermissionConstants.Dashboard,
            //    Name = PermissionConstants.Dashboard,
            //}

            //);
        }
    }
}