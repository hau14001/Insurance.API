namespace Insurance.Application.Common.Auth.Dtos
{
    public class JwtOptions
    {
        public int ExpiresInMinutes { get; set; }
        public string Secret { get; set; }
    }
}