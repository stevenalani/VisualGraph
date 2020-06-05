using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualGraph.Shared.Models
{
    public class UserModel
    {
        public string Id { get; set; } = "";
        public string Firstname { get; set; } = "";
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        [JsonIgnore]
        public string ErrorMessage { get; set; } = string.Empty;
        [JsonIgnore]
        public bool HasError => ErrorMessage != string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        [JsonIgnore]
        public bool IsAuth { get; set; } = false;


    }
    public class UserUpdateModel : UserModel
    {
        public static UserUpdateModel FromUserModel(UserModel usermodel) => new UserUpdateModel()
        {
            ErrorMessage = usermodel.ErrorMessage,
            Firstname = usermodel.Firstname,
            Id = usermodel.Id,
            IsAuth = usermodel.IsAuth,
            Name = usermodel.Name,
            NewPassword = "",
            Password = "",
            RetypedPassword = "",
            Roles = usermodel.Roles,
            Username = usermodel.Username,
        };
        public string NewPassword { get; set; } = "";
        public string RetypedPassword { get; set; } = "";
    }
}
