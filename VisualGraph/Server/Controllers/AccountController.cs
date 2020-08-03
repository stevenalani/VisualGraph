using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Server.Providers;
using VisualGraph.Server.Shared;
using VisualGraph.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VisualGraph.Server.Controllers
{
    /// <summary>
    /// Steuert Benutzer Accounts
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "IsVisualGraphMember")]
    public class AccountController : ControllerBase
    {
        private readonly UserProvider userProvider;
        private IConfiguration _config;
        /// <summary>
        /// Erstellt Instanz der Klasse
        /// </summary>
        /// <param name="config">Dependency Injection Configuration</param>
        public AccountController(IConfiguration config)
        {
            _config = config;
            userProvider = new UserProvider(_config.GetValue<string>("UserDir", "_users"));
        }
        /// <summary>
        /// Frägt den aktuellen Benutzer ab
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User, wenn eingeloggt. Sonst Gast Account</returns>
        // GET: api/<controller>
        [AllowAnonymous]
        [HttpGet("user/{username?}")]
        public async Task<UserModel> GetUser([FromRoute] string username)
        {
            UserModel user = new UserModel
            {
                Username = username ?? "Gast",
            };
            if (User != null && User.Identity.IsAuthenticated)
            {
                user = await userProvider.FindUser(User.Identity.Name);
                if(user == null)
                {
                    return new UserModel
                    {
                        Username = "Gast",
                    };
                }
                user.ErrorMessage = "";
                user.Password = "";
                user.IsAuth = true;
            }
            return user;
        }
        /// <summary>
        /// Sucht nach dem angegebenen Username und gibt den Benutzer zurück
        /// </summary>
        /// <param name="username">zu suchender User</param>
        /// <returns>gefundenen Benutzer</returns>
        [Authorize(Policy = "IsVisualGraphAdmin")]
        [HttpGet("userbyname/{username}")]
        public async Task<UserModel> GetUserByName([FromRoute] string username)
        {
            var user = await userProvider.FindUser(username);
            user.ErrorMessage = "";
            user.Password = "";
            return user;
        }
        /// <summary>
        /// Sucht nach allen registrierten Benutzern 
        /// </summary>
        /// <returns>Liste mit Benutzernamen</returns>
        [Authorize(Policy = "IsVisualGraphAdmin")]
        [HttpGet("usernames")]
        public async Task<List<string>> GetUsernames()
        {
            var usernames = await userProvider.GetUsernames();
            return usernames;
        }
        /// <summary>
        /// Gibt alle bekannten Rollen und die entsprechenden Rollennamen zurück
        /// </summary>
        /// <returns>Liste mit string array aus Rollen und Rollennamen</returns>
        [HttpGet("roles")]
        [AllowAnonymous]
        public Task<List<string[]>> GetRoles()
        {

            return Task.FromResult( new List<string[]>{
                new []{ "Admin", VGAppSettings.AdminRole },
                new []{ "Member",VGAppSettings.MemberRole },
                new []{ "SuperUser",VGAppSettings.SuperUserRole }
            });
            
        }
        /// <summary>
        /// Meldet den aktuellen Benutzer ab
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
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
        /// <summary>
        /// Meldet den Benutzer an, wenn die Benutzerdaten übereinstimmen
        /// </summary>
        /// <param name="userModel">UserModel des anzumeldenden Benutzers</param>
        /// <returns>Das übermittelte Usermodel. Bei Fehlern mit Fehlermeldung</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<UserModel> Login([FromBody] UserModel userModel)
        {
            if (User?.Identity.IsAuthenticated ?? false)
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
            else
            {
                userModel.ErrorMessage = "Falsches Passwort";
            }
            return userModel;

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
        /// <summary>
        /// Registriert einen Anwender
        /// </summary>
        /// <param name="userModel">Usermodel mit Benutzerdaten</param>
        /// <returns>Das übermittelte Usermodel. Bei Fehlern mit Fehlermeldung</returns>
        [HttpPost("register")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<UserModel> Register([FromBody] UserModel userModel)
        {
            if (userModel == null)
                return new UserModel()
                {
                    ErrorMessage = "Keine Benutzerdaten erhalten!"
                };
            var _userModel = await userProvider.FindUser(userModel.Username);

            if (_userModel == null)
            {
                PasswordHasher<UserModel> passwordHasher = new PasswordHasher<UserModel>();
                userModel.Password = passwordHasher.HashPassword(userModel, userModel.Password);
                _userModel = await userProvider.RegisterUser(userModel);
                await signIn(_userModel);
                userModel = _userModel;
            }
            else
            {
                return new UserModel()
                {
                    ErrorMessage = "Der Benutzername ist bereits vergeben"
                };
            }
            return userModel;
        }
        /// <summary>
        /// Ändert die Benutzerdaten 
        /// </summary>
        /// <param name="userModel">geänderter Benutzer</param>
        /// <returns>Das übermittelte Usermodel. Bei Fehlern mit Fehlermeldung</returns>
        [HttpPost("update")]
        [Produces("application/json")]
        public async Task<UserModel> UpdateUser([FromBody] UserUpdateModel userModel)
        {
            if (userModel == null)
            {
                return new UserModel()
                {
                    ErrorMessage = "Keine Benutzerdaten erhalten!"
                };
            }
            
            if (!User.IsInRole(VGAppSettings.AdminRole) || !User.IsInRole(VGAppSettings.SuperUserRole))
            {
                userModel.Roles.Remove(VGAppSettings.AdminRole);
                userModel.Roles.Remove(VGAppSettings.SuperUserRole); 
            }
            var _userModel = await userProvider.FindUser(userModel.Username);
            if (_userModel != null && _userModel.Id == userModel.Id)
            {
                if (userModel.NewPassword != "")
                {
                    if (nopasswordneeded(userModel) || verifyUser(_userModel, userModel.Password) == PasswordVerificationResult.Success)
                    {
                        userModel.Password = new PasswordHasher<UserModel>().HashPassword(userModel, userModel.NewPassword);
                    }
                    else
                    {
                        userModel.ErrorMessage = "Das eingegebene Passwort stimmt nicht!";
                        return userModel;
                    }
                }
                else
                {
                    userModel.Password = _userModel.Password;
                }
                await userProvider.UpdateUser(userModel);

            }
            userModel.RetypedPassword = userModel.NewPassword = userModel.Password = "";
            return userModel;
        }

        private PasswordVerificationResult verifyUser(UserModel user, string password)
        {
            PasswordHasher<UserModel> pwHasher = new PasswordHasher<UserModel>();
            var result = pwHasher.VerifyHashedPassword(user, user.Password, password);
            return result;
        }
        private bool nopasswordneeded(UserUpdateModel editmodel)
        {
            if (User.Identity.IsAuthenticated 
                && (User.IsInRole(VGAppSettings.AdminRole) || User.IsInRole(VGAppSettings.SuperUserRole)) && 
                editmodel.Username != User.Identity.Name )
            {
                return true;
            }
            return false;
        }

    }
}