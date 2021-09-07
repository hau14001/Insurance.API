using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Users
{
    public class UpdateUserEmailModel
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}