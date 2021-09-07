using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Auth
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public object ExtraProps { get; set; }
        public string Ip { get; set; }
    }
}