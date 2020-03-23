using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using VisualGraph.Data;
using VisualGraph.Data.Additional.Models;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VisualGraph.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserProvider userProvider;
        private IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
            userProvider = new UserProvider(_config.GetValue<string>("UserDir", "_users"));
        }
        // GET: api/<controller>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<UserModel> Get()
        {
            UserModel user = new UserModel() { };
            if (User.Identity.IsAuthenticated)
            {
                user = userProvider.FindUser(User.Identity.Name).Result;
                user.Password = "";
                user.IsAuth = true;
            }
            return user;
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }
            else
            {
                await HttpContext.SignOutAsync();
                return LocalRedirect("/");
            }


        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserModel>> Login([FromBody] UserModel userModel,string returnUrl)
        {

            if (userModel == null)
                return Ok(new UserModel()
                {
                    Name = "Gast",
                    Username = "",
                    ErrorMessage = "Keine Logindaten erhalten!"
                });

            var foundUser = userProvider.FindUser(userModel.Username).Result;
            if (foundUser == null)
                return Ok(new UserModel()
                {
                    Name = "Gast",
                    Username = userModel.Username,
                    ErrorMessage = "Kein Benutzerkonto gefunden!",

                });
            PasswordHasher<UserModel> pwHasher = new PasswordHasher<UserModel>();
            var result = pwHasher.VerifyHashedPassword(foundUser, foundUser.Password, userModel.Password);
            if (result == PasswordVerificationResult.Success)
            {
                userModel =  await signIn(foundUser);
            }
            return Ok(userModel);

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
                new Claim(ClaimTypes.Sid, foundUser.Id)
            };
            if (foundUser.Roles.Count == 0)
                claims.Add(new Claim(ClaimTypes.Role, "Gast"));
            foreach (var role in foundUser.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var claimsIdentity = new ClaimsIdentity(claims,"Basic");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties() { IsPersistent = true });
            //System.Console.WriteLine("result\r\n{0}\r\n{1}", result., result.Principal.Identity.Name);
            foundUser.Password = "";
            foundUser.ErrorMessage = "";
            foundUser.IsAuth = User.Identity.IsAuthenticated;
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
                _usrModel = await userProvider.RegisterUser(usrModel);
                await signIn(_usrModel);
            }
            return usrModel;
        }

    }
}