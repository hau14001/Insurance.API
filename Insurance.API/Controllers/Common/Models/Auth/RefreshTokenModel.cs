using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Auth
{
    public class RefreshTokenModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}