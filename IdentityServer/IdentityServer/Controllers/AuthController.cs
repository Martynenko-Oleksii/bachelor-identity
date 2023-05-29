using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        //private readonly ILogger<AuthController> logger;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IIdentityServerInteractionService interactionService;
        private readonly IConfiguration config;

        public AuthController(
            //ILogger<AuthController> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IIdentityServerInteractionService interactionService,
            IConfiguration config)
        {
            //this.logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.interactionService = interactionService;
            this.config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await signInManager.SignOutAsync();

            LogoutRequest logoutRequest = await interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return Redirect(this.config["IdentityServer:Client:DefaultRedirectUrl"]);
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, false, false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(loginViewModel.UserName);
                user.LastSignInTime = DateTime.UtcNow;
                await userManager.UpdateAsync(user);

                return Redirect(loginViewModel.ReturnUrl);
            }

            ModelState.AddModelError(string.Empty, "Invalid User Name or Password");

            return View(loginViewModel);
        }

        public IActionResult Cancel()
        {
            return Redirect(this.config["IdentityServer:Client:DefaultRedirectUrl"]);
        }
    }
}
