using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Services
{
    public interface IAccountService
    {
        public NavigationManager NavigationManager { get; }
        Task LogIn(UserModel userModel);
        Task LogOut();
        Task<UserModel> Register(UserModel userModel);
        Task<ClaimsPrincipal> GetUser();
        Task<UserModel> GetUserModel();
        Task<UserModel> GetUserModelFromName(string username);
        Task<UserUpdateModel> GetUserUpdateModel();

         Task<UserModel> UpdateUser(UserModel userModel);
        Task<bool> UserIsAuthenticated();

    }
}
