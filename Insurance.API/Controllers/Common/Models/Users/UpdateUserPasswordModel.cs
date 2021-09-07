using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Users
{
    public class UpdateUserPasswordModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}