using Main_SD;
using Microsoft.AspNetCore.Components.Authorization;
using Audit_Tracker_Blazor.Services.DB_Services;
using System.Security.Claims;

namespace Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider

    {
        public Admin_Services _services;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor, Admin_Services services)
        {
            _httpContextAccessor = httpContextAccessor;
            _services = services;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var identity = (ClaimsIdentity)user.Identity;

            if (identity != null && identity.IsAuthenticated)
            {
                var username = identity.Name;

                if (await ValidateAudit(username))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, SD.Auditor));
                }

                if (await ValidateMaster(username))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role,SD.Master));
                    identity.AddClaim(new Claim(ClaimTypes.Role, SD.Auditor));
                }
            }
            
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<bool> ValidateAudit(string username)
        {
            string sub = username.Substring(SD.UserDomain.Length);
            var user = await _services.GetAdmin(sub);
            if (user != null)
            {
                var role = await _services.GetRole(user.RoleID);
                if (role != null)
                {
                    if (role.Code == SD.Auditor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> ValidateMaster(string username)
        {
            string sub = username.Substring(SD.UserDomain.Length);
            var user = await _services.GetAdmin(sub);
            if (user != null)
            {
                var role = await _services.GetRole(user.RoleID);
                if (role != null)
                {
                    if (role.Code == SD.Master)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
