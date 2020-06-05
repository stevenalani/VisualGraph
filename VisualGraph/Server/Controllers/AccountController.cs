using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.JSInterop;

using VisualGraph.Shared.Models;
using VisualGraph.Server.Providers;
using VisualGraph.Server.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VisualGraph.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserProvider userProvider;
        private IConfiguration _config;
        private readonly IJSRuntime _JsRuntime;
        public AccountController(IConfiguration config, IJSRuntime jSRuntime)
        {
            _JsRuntime = jSRuntime;
            _config = config;
            userProvider = new UserProvider(_config.GetValue<string>("UserDir", "_users"));
        }

        // GET: api/<controller>
        [AllowAnonymous]
        [HttpGet("user/{username?}")]
        public async Task<UserModel> GetUser([FromRoute] string? username)
        {
            UserModel user = new UserModel
            {
                Username = username ?? "Gast",
            };
            if (User != null && User.Identity.IsAuthenticated)
            {
                user = await userProvider.FindUser(User.Identity.Name);
                user.Password = "";
                user.IsAuth = true;
            }
            return user;
        }
        [HttpGet("logout")]
        [Authorize(Policy = "IsVisualGraphMember")]
        public async Task<IActionResult> Logout()
        {

            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Ok("Kein Nutzer eingeloggt!");
            }
            else
            {
                await HttpContext.SignOutAsync();
                return Ok("ausgeloggt!");
            }
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<UserModel> Login([FromBody] UserModel userModel)
        {
            if (User?.Identity.IsAuthenticated??false)
            {
                await HttpContext.SignOutAsync();
            }
            if (userModel == null)
                return new UserModel()
                {
                    Name = "Gast",
                    Username = "",
                    ErrorMessage = "Keine Logindaten erhalten!"
                };

            var foundUser = await userProvider.FindUser(userModel.Username);
            if (foundUser == null)
                return new UserModel()
                {
                    Name = "Gast",
                    Username = userModel.Username,
                    Password = "",
                    ErrorMessage = "Kein Benutzerkonto gefunden!",

                };

            var result = verifyUser(foundUser, userModel.Password);
            if (result == PasswordVerificationResult.Success)
            {
                userModel = await signIn(foundUser);
                userModel.Password = "";
            }
            return userModel;

        }
        [HttpPost("verifieduser")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> verifieduser([FromBody] string userModel)
        {
            if (User.Identity.IsAuthenticated || userModel == null)
                return Ok(false);
            var foundUser = await userProvider.FindUser(userModel);
            if (foundUser == null)
                return Ok(false);

            await signIn(foundUser);
            return Ok(true);
        }

        private async Task<UserModel> signIn(UserModel foundUser)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, "cn=" + foundUser.Firstname),
                new Claim(ClaimTypes.Name, foundUser.Username),
                new Claim(ClaimTypes.AuthenticationMethod, "Local"),
                new Claim(ClaimTypes.NameIdentifier, foundUser.Id),
                new Claim(ClaimTypes.Email, foundUser.Username+"@visualgraph.eu"),
                new Claim(ClaimTypes.Sid, foundUser.Id),
                new Claim(ClaimTypes.Role, VGAppSettings.MemberRole)
            };

            foreach (var role in foundUser.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var claimsIdentity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties() { IsPersistent = true });
            foundUser.Password = "";
            foundUser.ErrorMessage = "";
            foundUser.IsAuth = true;
            return foundUser;
        }
        [HttpPost("register")]
        [Produces("application/json")]
        public async Task<UserModel> Register([FromBody] UserModel usrModel)
        {
            if (usrModel == null)
                return new UserModel()
                {
                    ErrorMessage = "Keine Benutzerdaten erhalten!"
                };
            var _usrModel = await userProvider.FindUser(usrModel.Username);

            if (_usrModel == null)
            {
                PasswordHasher<UserModel> passwordHasher = new PasswordHasher<UserModel>();
                usrModel.Password = passwordHasher.HashPassword(usrModel, usrModel.Password);
                _usrModel = await userProvider.RegisterUser(usrModel);
                await signIn(_usrModel);
            }
            return usrModel;
        }
        [HttpPost("update")]
        [Produces("application/json")]
        public async Task<UserModel> UpdateUser([FromBody] UserUpdateModel usrModel)
        {
            if (usrModel == null)
            {
                return new UserModel()
                {
                    ErrorMessage = "Keine Benutzerdaten erhalten!"
                };
            }
            var _usrModel = await userProvider.FindUser(usrModel.Username);
            if (_usrModel != null && _usrModel.Id == usrModel.Id)
            {
                if (usrModel.NewPassword != "")
                {
                    if (verifyUser(_usrModel, usrModel.Password) == PasswordVerificationResult.Success)
                    {
                        usrModel.Password = new PasswordHasher<UserModel>().HashPassword(usrModel, usrModel.NewPassword);
                    }
                    else
                    {
                        usrModel.ErrorMessage = "Der das eingegebene Passwort ist stimmt nicht!";
                        return usrModel;
                    }
                }
                else
                {
                    usrModel.Password = _usrModel.Password;
                }
                await userProvider.UpdateUser(usrModel);

            }
            return usrModel;
        }

        private PasswordVerificationResult verifyUser(UserModel user, string password)
        {
            PasswordHasher<UserModel> pwHasher = new PasswordHasher<UserModel>();
            var result = pwHasher.VerifyHashedPassword(user, user.Password, password);
            return result;
        }

    }
}