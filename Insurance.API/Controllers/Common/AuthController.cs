using System;
using System.Threading.Tasks;
using Insurance.API.Controllers.Common.Models.Auth;
using Insurance.Application.Common.Auth;
using Insurance.Application.Common.Auth.Dtos;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers.Common
{
    [ApiController]
    [Route("v1/api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<ServiceResponse> Login(LoginModel model)
        {
            if (model.Ip.IsNullOrEmpty())
            {
                model.Ip = GetIpAddress();
            }

            return await _authService.AuthenticateAsync(new AuthRequest
            {
                Password = model.Password,
                UserName = model.UserName,
                ExtraProps = model.ExtraProps,
                RemoteIpAddress = model.Ip
            });
        }

        [Authorize]
        [HttpGet("confirm-email")]
        public async Task<ServiceResponse> ConfirmEmail(string id, string token)
        {
            return await _authService.ConfirmEmail(id, token);
        }

        [Authorize]
        [HttpGet("confirm-phone")]
        public async Task<ServiceResponse> ConfirmPhone(string phone, string token)
        {
            return await _authService.ConfirmPhone(phone, token);
        }

        [HttpGet("resent-confirm-email/{email}")]
        public async Task<ServiceResponse> ResentConfirmEmail(string email)
        {
            return await _authService.ResentEmailConfirm(email);
        }

        [Authorize]
        [HttpGet("resent-confirm-phone/{phone}")]
        public async Task<ServiceResponse> ResentConfirmPhone(string phone)
        {
            return await _authService.ResentPhoneConfirm(phone);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ServiceResponse> RefreshToken(RefreshTokenModel model) => await _authService.RefreshTokenAsync(new RefreshTokenRequest
        {
            AccessToken = model.AccessToken,
            RefreshToken = model.RefreshToken,
            RemoteIpAdderss = GetIpAddress()
        });

        [Authorize]
        [HttpPost("validate")]
        public ServiceResponse ValidateToken(ValidateTokenModel model) => _authService.ValidateToken(model.Token);

        private string GetIpAddress()
        {
            return Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString()
                : HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}