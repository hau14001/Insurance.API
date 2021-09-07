using System.Threading.Tasks;
using Insurance.Application.Common.Auth.Dtos;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponse> AuthenticateAsync(AuthRequest request);

        Task<ServiceResponse> RefreshTokenAsync(RefreshTokenRequest request);

        ServiceResponse ValidateToken(string token);

        Task<ServiceResponse> ResentEmailConfirm(string request);

        Task<ServiceResponse> ResentPhoneConfirm(string phone);

        Task<ServiceResponse> ConfirmEmail(string id, string token);

        Task<ServiceResponse> ConfirmPhone(string phone, string token);
    }
}