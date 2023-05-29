using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProfileController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPut("SetInitialUserInfo")]
        public async Task<IActionResult> SetInitialUserInfo(ProfileInfo info)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(id);
            if (!user.UserConfirmed)
            {
                user.FirstName = info.FirstName;
                user.LastName = info.LastName;
                user.Email = info.Email;
                user.UserConfirmed = true;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Cannot set initial profile info");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                result = await _userManager.ResetPasswordAsync(user, token, info.Password);
                if (!result.Succeeded)
                {
                    return BadRequest("Cannot set new password. Please, try again later!");
                }

                return Ok(new { Updated = true });
            }

            return Ok(new { Updated = false });
        }

        [HttpPut("UpdateProfileInfo")]
        public async Task<IActionResult> UpdateProfileInfo(ProfileInfo info)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(id);
            user.FirstName = info.FirstName;
            user.LastName = info.LastName;
            user.Email = info.Email;
            user.UserConfirmed = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Cannot update profile info");
            }

            return Ok(new { Updated = true });
        }

        [HttpPut("UpdateUserPassword")]
        public async Task<IActionResult> UpdateUserPassword(ProfileInfo info)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);

            var result = await _userManager.ChangePasswordAsync(user, info.OldPassword, info.Password);
            if (!result.Succeeded)
            {
                return BadRequest("Cannot set new password. Please, try again later!");
            }

            return Ok(new { Updated = true });
        }
    }
}
