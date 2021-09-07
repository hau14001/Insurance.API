using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Roles
{
    public class UpdateRoleNameModel
    {
        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}