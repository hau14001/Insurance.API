using System.Collections.Generic;

namespace Insurance.Application.Common.Auth.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public List<string> Permissions { get; set; }
    }
}