using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuestBoard.Context;
using QuestBoard.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace QuestBoard_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _log;

        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        public AccountController(ILogger<AccountController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public class RegisterViewModel
        {
            [Required]
            [Display(Name = "Email")]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }

        public class LoginViewModel : RegisterViewModel
        {
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _log.LogInformation("User {} logged in successfully.", model.Email);
                return Ok(new { Success = true });
            }

            _log.LogWarning("User {} failed to login sucessfully.", model.Email);
            return BadRequest(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            User user = new User { UserName = model.Email, Email = model.Email };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _log.LogInformation("User {userName} was created.", model.Email);
                //Bug: Remember browser option missing?
                //Uncomment this and comment the later part if account verification is not needed.
                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(new { Success = true });
            }

            return UnprocessableEntity(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Success = true });
        }

        /*private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }*/
    }
}
