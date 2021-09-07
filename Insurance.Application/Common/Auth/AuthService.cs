using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Insurance.Application.Common.Auth.Dtos;
using Insurance.Application.Services;
using Insurance.Domain.Common;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.Data;
using Insurance.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Application.Common.Auth
{
    public class AuthService<TUser> : Service, IAuthService
        where TUser : User
    {
        private List<string> _emailAdmins = new List<string>() { "hau.bimspeed@gmail.com", "hau.bimspeed@bimspeed.vn" };
        private readonly IJwtHandler _jwtHandler;
        private readonly IRepository _repository;
        private readonly UserManager<TUser> _userManager;
        private readonly ICurrentUser _currentUser;

        public AuthService(IJwtHandler jwtHandler, IRepository repository, UserManager<TUser> userManager, ICurrentUser currentUser)
        {
            _jwtHandler = jwtHandler;
            _repository = repository;
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public virtual async Task<ServiceResponse> AuthenticateAsync(AuthRequest request)
        {
            var user = await _repository.FindAsync<TUser>(u => u.NormalizedUserName == request.UserName.ToUpper(), new List<string> { "RefreshTokens" });

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.BPassword))
            {
                return BadRequest("login_failure", "Invalid username or password.");
            }

            user.Ip = request.RemoteIpAddress;
            user.DeactiveActiveRefreshTokens();
            var refreshToken = _jwtHandler.GenerateRefreshToken();
            user.AddRefreshToken(refreshToken, request.RemoteIpAddress);
            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok(new AuthResponse
            {
                AccessToken = _jwtHandler.GenerateAccessToken(user.Id, user.UserName, request.RemoteIpAddress),
                RefreshToken = refreshToken,
                UserName = user.UserName,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneConfirmed = user.PhoneNumberConfirmed,
                Permissions = user.IsSystemUser ? new List<string>() { "ADMIN" } : await GetFullPermissions(user.Id)
            });
        }

        private async Task<List<string>> GetFullPermissions(Guid userId)
        {
            var permissions = _repository.GetQueryable<Permission>();
            var userPermissions = _repository.GetQueryable<UserPermission>();

            var pers1 = await userPermissions
                .Where(up => up.UserId == userId)
                .Join(permissions, userPermission => userPermission.PermissionId, permission => permission.Id, (userPermission, permission) => permission).ToListAsync();

            var roleIds = (await _repository.FindAllAsync<UserRole>(x => x.UserId == userId)).Select(x => x.RoleId);

            var list = new List<string>();
            list.AddRange(pers1.Select(x => x.Name));
            foreach (var roleId in roleIds)
            {
                var permissionIds =
                    (await _repository.FindAllAsync<RolePermission>(x => x.RoleId == roleId)).Select(x =>
                        x.PermissionId);
                var pers = await _repository.FindAllAsync<Permission>(x => permissionIds.Contains(x.Id));
                list.AddRange(pers.Select(x => x.Name));
            }

            list = list.Distinct().OrderBy(x => x).ToList();

            return list;
        }

        public virtual async Task<ServiceResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var failedResponse = BadRequest("refresh_token_failure", "Invalid token.");
            var claimsPrincipal = _jwtHandler.GetPrincipalFromToken(request.AccessToken, false);
            if (claimsPrincipal == null)
            {
                return failedResponse;
            }

            var userIdClaim = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            var user = await _repository.FindAsync<TUser>(new Guid(userIdClaim.Value), new List<string> { "RefreshTokens" });
            if (user == null || !user.HasValidRefreshToken(request.RefreshToken))
            {
                return failedResponse;
            }

            var at = _jwtHandler.GenerateAccessToken(user.Id, user.UserName, request.RemoteIpAdderss);
            return Ok(new AuthResponse
            {
                AccessToken = at
            });
        }

        public virtual ServiceResponse ValidateToken(string token)
        {
            var badRequest = BadRequest("token_invalid", "Invalid token");
            if (token.IsNullOrEmpty())
            {
                return badRequest;
            }

            var claims = _jwtHandler.GetPrincipalFromToken(token);
            if (claims == null)
            {
                return badRequest;
            }

            return Ok(claims.Claims.ToDictionary(claim => claim.Type, claim => claim.Value));
        }

        public async Task<ServiceResponse> ResentEmailConfirm(string request)
        {
            //var users = _repository.GetQueryable<User>();
            //foreach (var user1 in users)
            //{
            //    user1.PasswordHash = user1.BPassword;
            //    await _repository.UpdateAsync(user1);
            //}

            //await _repository.SaveChangeAsync();

            //return Ok();

            var user = await _userManager.FindByEmailAsync(request);
            if (user != null)
            {
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));

                var url = $"http://bimspeed.site/xac-thuc-email?id={user.Id}&&token={code}";

                //var body = GmailHelpers.GetBodyConFirmEmail("Customer", url);
                //_ = GmailHelpers.SentMail("Xác thực email", body, request, _emailAdmins);

                return Ok("Đã gửi email!");
            }

            return BadRequest("Not found", "Địa chỉ email này chưa đăng ký tài khoản");
        }

        public async Task<ServiceResponse> ResentPhoneConfirm(string phone)
        {
            if (_currentUser.GetId() == Guid.Empty)
            {
                return BadRequest("B", "Không tim thấy user");
            }

            var user = await _userManager.FindByIdAsync(_currentUser.GetId().ToString());

            if (user == null)
            {
                return BadRequest("B", "Không tim thấy user");
            }

            var isExist = await _repository.FindAsync<User>(x => x.PhoneNumber == phone && x.Id != user.Id) != null;
            if (isExist)
            {
                return BadRequest("B", "Số điện thoại này đã được đăng ký sử dụng");
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phone);

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://api.abenla.com/api/SendVoice?loginName=AB84GPX&sign=9df59e8da1dfea1bcd5891347e04352d&serviceTypeId=260&voiceMessageId=5856&phoneNumber=0979651347&ab_otp_code=" + token))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (apiResponse.Contains("106") || apiResponse.Contains("Success"))
                    {
                        return Ok();
                    }
                }
            }

            return BadRequest("B", "Gửi mã xác thực không thành bạn hãy liên hệ bộ phận CSKH nhé !");
        }

        public async Task<ServiceResponse> ConfirmEmail(string id, string token)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            {
                return BadRequest("B-Fail", "Confirm email fail");
            }

            if (Guid.TryParse(id, out var guidId))
            {
                var user = await _repository.FindAsync<TUser>(guidId);

                var encodeToken = WebEncoders.Base64UrlDecode(token);
                var tks = Encoding.UTF8.GetString(encodeToken);
                var rs = await _userManager.ConfirmEmailAsync(user, tks);
                if (rs.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest("B-Fail", "Confirm email fail");
        }

        public async Task<ServiceResponse> ConfirmPhone(string phone, string token)
        {
            if (_currentUser.GetId() == Guid.Empty)
            {
                return BadRequest("B", "Không tim thấy user");
            }
            var user = await _userManager.FindByIdAsync(_currentUser.GetId().ToString());

            var rs = await _userManager.ChangePhoneNumberAsync(user, phone, token);

            if (rs.Succeeded)
            {
                return Ok();
            }

            return BadRequest("B-Fail", rs.Errors.FirstOrDefault()?.Description);
        }
    }
}