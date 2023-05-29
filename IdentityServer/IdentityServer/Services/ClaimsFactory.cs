using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityServer.Services
{
    public class ClaimsFactory : UserClaimsPrincipalFactory<User>
    {
        private readonly UserManager<User> _userManager;

        public ClaimsFactory(
            UserManager<User> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            identity.AddClaim(new Claim("customerId", user.CustomerId.ToString(), ClaimValueTypes.Integer32));
            identity.AddClaim(new Claim("email", (user.Email ?? "")));
            identity.AddClaim(new Claim("firstName", (user.FirstName ?? "")));
            identity.AddClaim(new Claim("lastName", (user.LastName ?? "")));
            identity.AddClaim(new Claim("customerName", user.CustomerName));
            identity.AddClaim(new Claim("lastSignIn", user.LastSignInTime.ToString(), ClaimValueTypes.DateTime));
            identity.AddClaim(new Claim("confirmed", user.UserConfirmed.ToString(), ClaimValueTypes.Boolean));

            return identity;
        }
    }
}
