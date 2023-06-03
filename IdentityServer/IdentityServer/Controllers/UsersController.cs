using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("GetUsersInfo/{customerId}")]
        public async Task<IActionResult> GetUsersInfo(int customerId)
        {
            var userCustomerId = User.FindFirstValue("customerId");
            if (Int32.TryParse(userCustomerId, out int value))
            {
                if (value == customerId)
                {

                    var users = await _context.Users
                        .Where(x => x.CustomerId == customerId)
                        .Select(x => new
                        {
                            x.Id,
                            x.UserName,
                            FullName = $"{x.LastName}, {x.FirstName}",
                            SignInTime = x.LastSignInTime,
                            x.Email,
                        })
                        .ToListAsync();

                    return Ok(users);
                }
            }

            return BadRequest("You have not access to this Customer");
        }

        [HttpGet("GetUserRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            var userCustomerId = User.FindFirstValue("customerId");
            if (Int32.TryParse(userCustomerId, out int value))
            {
                var user = await _context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
                if (user is null) return BadRequest("There is no such User");

                if (user.CustomerId == value)
                {
                    var userRoles = await _context.UserRoles
                        .Where(x => x.UserId.Equals(userId))
                        .Select(x => new
                        {
                            Id = x.RoleId
                        })
                        .ToListAsync();

                    return Ok(userRoles);
                }
            }

            return BadRequest("You have not access to Users from this Customer");
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPut("ResetUserPassword/{userId}")]
        public async Task<IActionResult> ResetUserPassword(string userId, PasswordInfo passwordInfo)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id.Equals(userId)) return BadRequest("You cannot reset own password here");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return BadRequest("There is no such user");

            var userCustomerId = User.FindFirstValue("customerId");
            if (int.TryParse(userCustomerId, out int value))
            {
                if (user.CustomerId != value) return BadRequest("You have not access to this user");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, passwordInfo.Password);
            if (!result.Succeeded)
            {
                return BadRequest("Cannot set new password. Please, try again later!");
            }

            return Ok();
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterNewUser(NewUserDto userInfo)
        {
            var userCustomerId = User.FindFirstValue("customerId");
            if (int.TryParse(userCustomerId, out int value))
            {
                if (value != 6 && userInfo.CustomerId != value) return BadRequest("You cannot add a user to this customer");
            }

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new User()
            {
                CustomerId = userInfo.CustomerId,
                CustomerName = userInfo.CustomerName,
                UserName = userInfo.UserName,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
            };

            var result = await _userManager.CreateAsync(user, userInfo.Password);
            if (!result.Succeeded) return BadRequest("Something went wrong. Try it again later");

            result = await _userManager.AddToRolesAsync(user, userInfo.Roles.Select(x => x.Name));
            if (!result.Succeeded) return BadRequest("Something went wrong. Try it again later");

            return Ok(new
            {
                user.Id,
                user.UserName,
                FullName = $"{user.LastName}, {user.FirstName}",
                user.Email,
            });
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id.Equals(userId)) return BadRequest("You cannot delete yourself");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return BadRequest("There is no such user");

            var userCustomerId = User.FindFirstValue("customerId");
            if (int.TryParse(userCustomerId, out int value))
            {
                if (user.CustomerId != value) return BadRequest("You have not access to this user");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest("Something went wrong/ Trry it later");

            return Ok();
        }
    }
}
