using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Auth
{
    public class ValidateTokenModel
    {
        [Required]
        public string Token { get; set; }
    }
}